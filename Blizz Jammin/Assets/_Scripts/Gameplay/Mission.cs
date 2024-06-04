using _Scripts.Schemas;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Mission : MonoBehaviour, ISchemaController<SchemaMission>
    {
        /// <summary>
        /// TEMP: A Mission can be serialized with a specific instance of data on startup.
        /// </summary>
        [SerializeField] private SchemaMission m_data;

        private void Awake()
        {
            if (m_data != null)
            {
                SetData(m_data);
            }
        }
        
        [SerializeField] private SpriteRenderer m_icon;
            
        public void SetData(SchemaMission data)
        {
            m_data = data;
            m_icon.sprite = data.Icon;
        }
        
        private void OnMouseDown()
        {
            UIPopup popup = ServiceLocator.Instance.UIPopupManager.GetPopup(UIPopupManager.PopupType.MissionDetails);
            UIMissionDetails missionDetails = popup.GetComponent<UIMissionDetails>();
            missionDetails.SetData(m_data);
            
            ServiceLocator.Instance.UIPopupManager.RequestPopup(UIPopupManager.PopupType.MissionDetails);
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
