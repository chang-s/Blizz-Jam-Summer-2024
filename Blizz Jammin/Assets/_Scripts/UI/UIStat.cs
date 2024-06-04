using _Scripts.Schemas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIStat : MonoBehaviour, ISchemaController<SchemaStat>
    {
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_amount;
        
        public void SetData(SchemaStat data)
        {
            m_icon.sprite = data.Sprite;
        }
        
        public void SetAmount(int amount)
        {
            m_amount.SetText(amount.ToString());
        }
    }
}
