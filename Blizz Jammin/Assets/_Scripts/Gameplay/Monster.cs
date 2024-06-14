using System;
using _Scripts.Schemas;
using _Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Monster : MonoBehaviour, ISchemaController<SchemaMonster>
    {
        public SchemaMonster Data { get; private set; }
        public uint Level { get; private set; } = 1;

        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private TextMeshPro m_nameLabel;
        
        public void SetData(SchemaMonster data)
        {
            Data = data;
            
            m_nameLabel.SetText(data.Name);
            m_spriteRenderer.transform.localScale = data.Scale;
            m_spriteRenderer.sprite = data.Sprite;
        }

        public int GetStatValue(SchemaStat stat)
        {
            int baseValue = Data.Stats[stat];
            return (int) (baseValue * Math.Pow(1.1f, Level - 1));
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
            monsterDetails.SetData(Data);
            
            popupManager.RequestPopup(UIPopupManager.PopupType.MonsterDetails);
        }
    }
}
