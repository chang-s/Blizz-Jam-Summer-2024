using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Mission")]
    public class SchemaMission : Schema
    {
        [Serializable]
        public struct Modifier
        {
            public SchemaQuirk Quirk;
            
            [Range(-3, 3)]
            public int ModValue;
        }
        
        [BoxGroup("Visuals")]
        public string Name;
        
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite Icon;

        [BoxGroup("Simulation")] 
        [Range(1, 6)]
        public int MaxCapacity;
        
        [BoxGroup("Simulation")]
        public int Endurance;
        
        [BoxGroup("Simulation")]
        public int Days;

        [BoxGroup("Simulation")] 
        public int ModifierCount;
        
        [BoxGroup("Simulation")] 
        public Modifier[] Modifiers;

        [BoxGroup("Simulation")]
        public Modifier[] ClassModifiers;

        [BoxGroup("Rewards")] 
        public SchemaLootTable LootTable;

        [BoxGroup("Rewards")] 
        public float Infamy;
        
        [BoxGroup("Rewards")] 
        public float Xp;

        [BoxGroup("Rewards")]
        public string Difficulty;

        [BoxGroup("Rewards")]
        public string Quote;
    }
}
