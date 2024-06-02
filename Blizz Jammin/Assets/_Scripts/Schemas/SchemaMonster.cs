using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Monster")]
    public class SchemaMonster : Schema
    {
        /// <summary>
        /// The monster's name to be displayed in UI.
        /// </summary>
        [BoxGroup("Visuals")]
        public string Name;
        
        /// <summary>
        /// The sprite used when shown in the world/UI.
        /// </summary>
        [BoxGroup("Visuals")]
        public Vector3 m_scale;
        
        /// <summary>
        /// The sprite used when shown in the world/UI.
        /// </summary>
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite m_sprite;

        /// <summary>
        /// The monster's base stats.
        /// </summary>
        public Dictionary<SchemaStat, int> m_stats = new Dictionary<SchemaStat, int>();
        
        
        [Button("Add All Stats")]
        public void AddAllStats()
        {
            string[] allStatsGuids = AssetDatabase.FindAssets("t:SchemaStat");
            foreach (string statGuid in allStatsGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(statGuid);
                SchemaStat stat = AssetDatabase.LoadAssetAtPath<SchemaStat>(assetPath);
                if (m_stats.ContainsKey(stat))
                {
                    continue;
                }

                m_stats.Add(stat, 1);
            }
        }
    }

    
}
