using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/LootTable")]
    public class SchemaLootTable : Schema
    {
        [Serializable]
        public struct LootTableEntry
        {
            public float RequiredMissionRewardScalar;
            public int Rolls;
            public SchemaLoot[] RequiredLoot;
            public SchemaLoot[] PossibleLoot;
        }
        
        public LootTableEntry[] LootTable;
        
        /// <summary>
        /// Returns all unique loot entries that are possible with this loot table.
        /// </summary>
        /// <returns></returns>
        public HashSet<SchemaLoot> GetAllPossibleLoot()
        {
            HashSet<SchemaLoot> allLoot = new HashSet<SchemaLoot>();
            foreach (var lootTableEntry in LootTable)
            {
                if(lootTableEntry.RequiredLoot != null)
                    allLoot.AddRange(lootTableEntry.RequiredLoot);
                if(lootTableEntry.PossibleLoot != null)
                    allLoot.AddRange(lootTableEntry.PossibleLoot);
            }
            return allLoot;
        }
        
        /// <summary>
        /// Helper function for setting up data in edit time.
        /// </summary>
        [Button("Set Basic Loot Table (Empty)")]
        public void SetupBasicLootTable()
        {
            LootTable = new LootTableEntry[] {
                new LootTableEntry()
                {
                    Rolls = 2,
                    RequiredMissionRewardScalar = 1.0f,
                    RequiredLoot = new SchemaLoot[] {},
                    PossibleLoot = new SchemaLoot[] {}
                },
                new LootTableEntry()
                {
                    Rolls = 1,
                    RequiredMissionRewardScalar = 0.75f,
                    RequiredLoot = new SchemaLoot[] {},
                    PossibleLoot = new SchemaLoot[] {}
                },
                new LootTableEntry()
                {
                    Rolls = 1,
                    RequiredMissionRewardScalar = 0.5f,
                    RequiredLoot = new SchemaLoot[] {},
                    PossibleLoot = new SchemaLoot[] {}
                },
            };
        }
    }
}
