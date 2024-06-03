using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
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
        /// The monster's description to be displayed in UI.
        /// </summary>
        [BoxGroup("Visuals")]
        public string Description;
        
        /// <summary>
        /// The sprite used when shown in the world/UI.
        /// </summary>
        [BoxGroup("Visuals")]
        public Vector3 Scale;
        
        /// <summary>
        /// The sprite used when shown in the world/UI.
        /// </summary>
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite Sprite;

        // TODO: Expand this per-level, probably an intermediate ScriptableObject
        /// <summary>
        /// The monster's base stats.
        /// </summary>
        [BoxGroup("Behavior")]
        public Dictionary<SchemaStat, int> Stats = new Dictionary<SchemaStat, int>();
        
        [Button("Add All Stats")]
        public void AddAllStats()
        {
            string[] allStatsGuids = AssetDatabase.FindAssets("t:SchemaStat");
            foreach (string statGuid in allStatsGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(statGuid);
                SchemaStat stat = AssetDatabase.LoadAssetAtPath<SchemaStat>(assetPath);
                if (Stats.ContainsKey(stat))
                {
                    continue;
                }

                Stats.Add(stat, 1);
            }
        }
    }
}
