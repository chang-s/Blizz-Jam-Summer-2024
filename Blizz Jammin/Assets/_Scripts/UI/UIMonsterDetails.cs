using System.Collections.Generic;
using _Scripts.Schemas;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMonsterDetails : MonoBehaviour, ISchemaController<SchemaMonster>
    {
        [SerializeField] [CanBeNull] private Transform m_statRoot;
        [SerializeField] [CanBeNull] private TMP_Text m_name;
        [SerializeField] [CanBeNull] private TMP_Text m_description;
        [SerializeField] [CanBeNull] private Image m_icon;
        [SerializeField] private UIStat m_prefab;

        private List<UIStat> m_statInstances = new List<UIStat>();

        public void SetData(SchemaMonster data)
        {
            if (data == null)
            {
                return;
            }
            
            // Handle the image
            if (m_icon != null)
            {
                m_icon.sprite = data.Sprite;
            }

            // Handle texts
            m_name?.SetText(data.Name);
            m_description?.SetText(data.Description);

            if (m_statRoot != null)
            {
                // Handle stats
                // todo: recycle/pool elements. for now, just delete and remake
                foreach (var statInstance in m_statInstances)
                {
                    Destroy(statInstance.gameObject);
                }
                m_statInstances.Clear();

                foreach (var (statSchema, value) in data.Stats)
                {
                    UIStat instance = Instantiate(m_prefab, m_statRoot);
                    instance.SetData(statSchema);
                    instance.SetAmount(value);
                    m_statInstances.Add(instance);
                }
            }
        }
    }
}
