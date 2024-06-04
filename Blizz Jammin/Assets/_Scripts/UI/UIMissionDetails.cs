using _Scripts.Schemas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMissionDetails : MonoBehaviour, ISchemaController<SchemaMission>
    {
        // Mission details
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private Image m_icon;
        
        // Party
        [SerializeField] private Sprite m_addMonster;
        [SerializeField] private Button[] m_monsterButtons;
        
        // Rewards
        
        public void SetData(SchemaMission data)
        {
            m_name.SetText(data.Name);
            m_icon.sprite = data.Icon;
        }
    }
}
