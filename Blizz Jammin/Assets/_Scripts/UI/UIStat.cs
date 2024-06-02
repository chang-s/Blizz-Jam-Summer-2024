using _Scripts.Schemas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIStat : MonoBehaviour, ISchemaController
    {
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_amount;
        
        public void SetData(Schema schema)
        {
            SchemaStat data = schema as SchemaStat;
            if (data == null)
            {
                return;
            }

            m_icon.sprite = data.Sprite;
        }

        public void SetAmount(int amount)
        {
            m_amount.SetText(amount.ToString());
        }
    }
}
