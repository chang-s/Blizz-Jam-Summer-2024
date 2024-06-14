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
        
        [BoxGroup("Visuals")]
        public string Quote;

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
        
        // This is just UX temp. There is no string denoting the difficulty of the mission
        // What should be shown is the Endurance (or Health, whatever we end up calling it)
        // If we wanna call it difficulty, sure, but its going to be an Integer
        // If we want to do a string for this, then we should derive it from a mapping for Endurance -> Difficulty String?
        // I would much rather show the number though
        //[BoxGroup("Simulation")]
        //public string Difficulty;
    }
}
