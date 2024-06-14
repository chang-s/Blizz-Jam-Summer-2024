using _Scripts.Schemas;
using _Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Mission : MonoBehaviour, ISchemaController<SchemaMission>
    {
        [BoxGroup("Base")] 
        [SerializeField] private SpriteRenderer m_icon;
        
        [BoxGroup("States")] 
        [SerializeField] private GameObject m_combatGroup;
        [BoxGroup("States")] 
        [SerializeField] private GameObject m_completedGroup;

        private SchemaMission m_data;
        private MissionManager.MissionStatus m_status;
        
        public void SetData(SchemaMission data)
        {
            m_data = data;
            m_icon.sprite = data.Icon;
        }
        
        private void Awake()
        {
            ServiceLocator.Instance.MissionManager.OnMissionStatusChanged += OnMissionStatusChanged;
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
                var resultsPopup = popupManager.GetPopup(UIPopupManager.PopupType.MissionResults).GetComponent<UIMissionResults>();
                resultsPopup.SetData(m_data);
                popupManager.RequestPopup(UIPopupManager.PopupType.MissionResults);
                return;
            }

            // Otherwise, open the mission details
            UIPopup popup = popupManager.GetPopup(UIPopupManager.PopupType.MissionDetails);
            UIMissionDetails missionDetails = popup.GetComponent<UIMissionDetails>();
            missionDetails.SetData(m_data);
            popupManager.RequestPopup(UIPopupManager.PopupType.MissionDetails);
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
