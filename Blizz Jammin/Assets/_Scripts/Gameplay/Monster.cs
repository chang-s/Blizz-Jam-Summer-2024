using System;
using _Scripts.Schemas;
using TMPro;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Monster : MonoBehaviour, ISchemaController
    {
        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private TextMeshPro m_nameLabel;
        
        
        /// <summary>
        /// A Monster can be serialized with a specific instance of data on startup.
        /// </summary>
        [SerializeField] private SchemaMonster m_data;

        private void Awake()
        {
            if (m_data != null)
            {
                SetData(m_data);
            }
        }

        public void SetData(Schema schema)
        {
            SchemaMonster data = schema as SchemaMonster;
            if (data == null)
            {
                return;
            }

            m_nameLabel.SetText(data.Name);

            m_spriteRenderer.transform.localScale = data.m_scale;
            m_spriteRenderer.sprite = data.m_sprite;
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
