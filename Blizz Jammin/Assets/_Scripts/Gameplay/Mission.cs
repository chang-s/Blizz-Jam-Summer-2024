using System.Collections.Generic;
using _Scripts.Schemas;
using _Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Mission : WorldInstance, ISchemaController<SchemaMission>
    {
        public struct Modifier
        {
            public SchemaQuirk Quirk;
            
            [Range(-3, 3)]
            public int ModValue;
        }
        
        [BoxGroup("Visuals")] 
        [SerializeField] private SpriteRenderer m_icon;
        
        [BoxGroup("Visuals")] 
        [SerializeField] private Dictionary<MissionManager.MissionStatus, GameObject> m_states;

        public SchemaMission Data { get; private set; }
        public IReadOnlyCollection<Modifier> Modifiers => m_modifiers;
        
        private List<Modifier> m_modifiers;
        
        public void SetData(SchemaMission data)
        {
            Data = data;
            m_icon.sprite = data.Icon;

            m_modifiers = RollModifiers(data);
        }

        private void Awake()
        {
            ServiceLocator.Instance.MissionManager.OnMissionStatusChanged += OnMissionStatusChanged;
        }

        private List<Modifier> RollModifiers(SchemaMission data)
        {
            // We are safe to assume that this sum will always be less than or equal to the amount of total quirks
            var mods = new List<Modifier>(data.PossiblePositiveQuirkCount + data.PossibleNegativeQuirkCount);

            var allQuirks = ServiceLocator.Instance.AllQuirks;
            HashSet<SchemaQuirk> quirksUsed = new HashSet<SchemaQuirk>();

            int quirksToRoll = data.PossiblePositiveQuirkCount;
            while (quirksToRoll > 0)
            {
                var rolledQuirk = allQuirks[Random.Range(0, allQuirks.Length)];
                if (quirksUsed.Add(rolledQuirk))
                {
                    mods.Add(new Modifier()
                    {
                        Quirk = rolledQuirk,
                        ModValue = Random.Range(data.PositiveQuirkMinimumMod, data.PositiveQuirkMaximumMod)
                    });
                    quirksToRoll--;
                }
            }

            quirksToRoll = data.PossibleNegativeQuirkCount;
            while (quirksToRoll > 0)
            {
                var rolledQuirk = allQuirks[Random.Range(0, allQuirks.Length)];
                if (quirksUsed.Add(rolledQuirk))
                {
                    mods.Add(new Modifier()
                    {
                        Quirk = rolledQuirk,
                        ModValue = Random.Range(data.NegativeQuirkMinimumMod, data.NegativeQuirkMaximumMod)
                    });
                    quirksToRoll--;
                }
            }

            return mods;
        }
        
        private void OnMissionStatusChanged(MissionManager.MissionInfo missionInfo)
        {
            if (missionInfo.m_mission.Data != Data)
            {
                return;
            }
            
            SetStatus(missionInfo.m_status);
        }

        private void SetStatus(MissionManager.MissionStatus status)
        {
            foreach (var (missionStatus, group) in m_states)
            {
                group.SetActive(missionStatus == status);
            }
        }
        
        private void OnMouseDown()
        {
            // If we're already showing something, then disregard the click
            var popupManager = ServiceLocator.Instance.UIPopupManager;
            if (popupManager.HasActivePopup())
            {
                return;
            }
            
            var missionInfo = ServiceLocator.Instance.MissionManager.GetMissionInfo(Data);
            switch (missionInfo.m_status)
            {
                // If we're locked, then do nothing
                case MissionManager.MissionStatus.Locked:
                    return;
                
                // When clicking a complete mission, open the results popup
                case MissionManager.MissionStatus.Complete:
                    var resultsPopup = popupManager.GetPopup(SchemaPopup.PopupType.MissionResults).GetComponent<UIMissionResults>();
                    resultsPopup.SetData(Data);
                    popupManager.RequestPopup(SchemaPopup.PopupType.MissionResults);
                    break;

                // Otherwise, open the mission details
                default:
                    UIPopup popup = popupManager.GetPopup(SchemaPopup.PopupType.MissionDetails);
                    UIMissionDetails missionDetails = popup.GetComponent<UIMissionDetails>();
                    missionDetails.SetData(Data);
                    popupManager.RequestPopup(SchemaPopup.PopupType.MissionDetails);
                    break;
            }
        }
    }
}
