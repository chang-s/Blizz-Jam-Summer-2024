using System.Collections.Generic;
using _Scripts.Gameplay;
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
        /// The monster's class. This is a glorified Quirk, but can only have ONE of these from the set of 3.
        /// </summary>
        [BoxGroup("Behavior")]
        public SchemaQuirk Class;

        /// <summary>
        /// The amount of quirks to roll at the beginning of the game for this monster.
        /// All monsters have a chance to get any quirk. When we boot the game, each one will
        /// roll this many. 
        /// </summary>
        [BoxGroup("Behavior")]
        public int InitialQuirkCount;

        /// <summary>
        /// Determines if the player gets this monster at the start of the game.
        /// </summary>
        [BoxGroup("Behavior")]
        public Monster.MonsterStatus StartStatus = Monster.MonsterStatus.Locked;
        
        /// <summary>
        /// The monster's base stats. This gets scaled by level, see GameSettings.StatExponent
        /// </summary>
        [BoxGroup("Behavior")]
        public Dictionary<SchemaStat, int> Stats = new Dictionary<SchemaStat, int>();

        /// <summary>
        /// How much it costs to recruit this monster.
        /// </summary>
        [BoxGroup("Recruit")] 
        public int Cost;
        
        /// <summary>
        /// If provided, completing this mission with 100% will unlock the monster in the shop. 
        /// </summary>
        [BoxGroup("Recruit")] 
        public SchemaMission UnlockMission;
        
        /*
        /// <summary>
        /// If provided, getting this loot will unlock the monster in the shop. 
        /// </summary>
        [BoxGroup("Recruit")] 
        public SchemaLoot UnlockLoot;
        */
        
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
