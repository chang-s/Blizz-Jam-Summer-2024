using _Scripts.Schemas;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UILoot : MonoBehaviour, ISchemaController<SchemaLoot>
    {
        // Required
        [SerializeField] private Image m_icon;

        // Optional
        [SerializeField] [CanBeNull] private TMP_Text m_name;
        [SerializeField] [CanBeNull] private TMP_Text m_description;
        
        public void SetData(SchemaLoot data)
        {
            m_icon.sprite = data.Icon;
            
            m_name?.SetText(data.Name);
            m_description?.SetText(data.Description);
        }
    }
}
