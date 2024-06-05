using _Scripts.Schemas;
using _Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Monster : MonoBehaviour, ISchemaController<SchemaMonster>
    {
        public SchemaMonster Data => m_data;

        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private TextMeshPro m_nameLabel;
        
        
        /// <summary>
        /// TEMP: A Monster can be serialized with a specific instance of data on startup.
        /// </summary>
        [SerializeField] private SchemaMonster m_data;

        private void Awake()
        {
            if (m_data != null)
            {
                SetData(m_data);
            }
        }

        public void SetData(SchemaMonster data)
        {
            m_data = data;
            
            m_nameLabel.SetText(data.Name);
            m_spriteRenderer.transform.localScale = data.Scale;
            m_spriteRenderer.sprite = data.Sprite;
        }

        private void OnMouseDown()
        {
            // If we're already showing something, then disregard the click
            var popupManager = ServiceLocator.Instance.UIPopupManager;
            if (popupManager.HasActivePopup())
            {
                return;
            }
            
            UIPopup popup = popupManager.GetPopup(UIPopupManager.PopupType.MonsterDetails);
            UIMonsterDetails monsterDetails = popup.GetComponent<UIMonsterDetails>();
            monsterDetails.SetData(m_data);
            
            popupManager.RequestPopup(UIPopupManager.PopupType.MonsterDetails);
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
