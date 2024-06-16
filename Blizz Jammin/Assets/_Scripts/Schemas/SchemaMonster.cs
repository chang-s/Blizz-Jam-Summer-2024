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

        /// <summary>
        /// The amount of quirks to roll at the beginning of the game for this monster.
        /// </summary>
        public int QuirkCount;

        public SchemaQuirk[] PossibleClasses;

        /// <summary>
        /// The pool of quirks that can apply to this monster. Assume this is >= QuirkCount.
        /// TODO: Stretch goal, when leveling up to certain milestones, add quirk?
        /// </summary>
        public SchemaQuirk[] PossibleQuirks;

        /// <summary>
        /// Determines if the player gets this monster at the start of the game.
        /// </summary>
        [BoxGroup("Behavior")]
        public bool IsStarter = false;
        
        // TODO: Expand this per-level, probably an intermediate ScriptableObject?
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
