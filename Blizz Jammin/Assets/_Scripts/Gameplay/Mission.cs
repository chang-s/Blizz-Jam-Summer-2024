using System.Collections.Generic;
using _Scripts.Schemas;
using _Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Mission : MonoBehaviour, ISchemaController<SchemaMission>
    {
        public struct Modifier
        {
            public SchemaQuirk Quirk;
            
            [Range(-3, 3)]
            public int ModValue;
        }
        
        [BoxGroup("Base")] 
        [SerializeField] private SpriteRenderer m_icon;
        
        [BoxGroup("States")] 
        [SerializeField] private GameObject m_combatGroup;
        [BoxGroup("States")] 
        [SerializeField] private GameObject m_completedGroup;

        public IReadOnlyCollection<Modifier> Modifiers => m_modifiers;
        
        private SchemaMission m_data;
        private MissionManager.MissionStatus m_status;
        private List<Modifier> m_modifiers;
        
        public void SetData(SchemaMission data)
        {
            m_data = data;
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
            if (missionInfo.m_mission != m_data)
            {
                return;
            }
            
            SetStatus(missionInfo.m_status);
        }

        private void SetStatus(MissionManager.MissionStatus status)
        {
            m_status = status;
            
            //m_lockedGroup.SetActive(status == MissionManager.MissionStatus.Locked);
            m_combatGroup.SetActive(status == MissionManager.MissionStatus.InCombat);
            m_completedGroup.SetActive(status == MissionManager.MissionStatus.Complete);
        }
        
        private void OnMouseDown()
        {
            // If we're already showing something, then disregard the click
            var popupManager = ServiceLocator.Instance.UIPopupManager;
            if (popupManager.HasActivePopup())
            {
                return;
            }

            // When clicking a complete mission, open the results popup
            if (m_status == MissionManager.MissionStatus.Complete)
            {
                var resultsPopup = popupManager.GetPopup(SchemaPopup.PopupType.MissionResults).GetComponent<UIMissionResults>();
                resultsPopup.SetData(m_data);
                popupManager.RequestPopup(SchemaPopup.PopupType.MissionResults);
                return;
            }

            // Otherwise, open the mission details
            UIPopup popup = popupManager.GetPopup(SchemaPopup.PopupType.MissionDetails);
            UIMissionDetails missionDetails = popup.GetComponent<UIMissionDetails>();
            missionDetails.SetData(m_data);
            popupManager.RequestPopup(SchemaPopup.PopupType.MissionDetails);
        }

        private void OnValidate()
        {
            if (m_data != null)
            {
                SetData(m_data);
            }
        }
    }
}
