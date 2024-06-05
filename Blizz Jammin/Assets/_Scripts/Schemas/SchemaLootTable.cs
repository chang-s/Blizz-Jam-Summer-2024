using System;
using Sirenix.OdinInspector;
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
            public SchemaLoot[] PossibleLoot;
        }
        
        public LootTableEntry[] LootTable;
        
        [Button("Set Basic Loot Table (Empty)")]
        public void SetupBasicLootTable()
        {
            LootTable = new LootTableEntry[] {
                new LootTableEntry()
                {
                    Rolls = 2,
                    RequiredMissionRewardScalar = 1.0f,
                    PossibleLoot = new SchemaLoot[] {}
                },
                new LootTableEntry()
                {
                    Rolls = 1,
                    RequiredMissionRewardScalar = 0.75f,
                    PossibleLoot = new SchemaLoot[] {}
                },
                new LootTableEntry()
                {
                    Rolls = 1,
                    RequiredMissionRewardScalar = 0.5f,
                    PossibleLoot = new SchemaLoot[] {}
                },
            };
        }
    }
}
