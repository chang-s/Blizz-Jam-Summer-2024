using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/StatTable")]
    public class SchemaStatTable: Schema
    {
        /// <summary>
        /// The monster's base stats.
        /// </summary>
        [BoxGroup("Behavior")]
        public Dictionary<SchemaStat, int> Stats = new Dictionary<SchemaStat, int>();
        
        [Button("Add All Missing Stats (0)")]
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

                Stats.Add(stat, 0);
            }
        }
    }
}
