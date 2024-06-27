using _Scripts.Schemas;
using UnityEngine;

namespace _Scripts.UI.Tooltip
{
    public class UITooltipRequesterSchema : UITooltipRequester
    {
        // If the schema is static, use this
        [SerializeField] private Schema m_schema;

        /// <summary>
        /// If you need the schema to change dynamically, use this.
        /// </summary>
        public void SetSchema(Schema schema)
        {
            m_schema = schema;
        }
        
        public override string GetText()
        {
            return m_schema.GetTooltipText();
        }
    }
}