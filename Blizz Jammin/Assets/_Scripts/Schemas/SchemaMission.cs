using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Mission")]
    public class SchemaMission : Schema
    {
        [BoxGroup("Visuals")]
        public string Name;
        
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite Icon;
        
        [BoxGroup("Visuals")]
        public string Quote;

        [BoxGroup("Simulation")] 
        [Range(0, 3)]
        public int PossiblePositiveQuirkCount;
        
        [BoxGroup("Simulation")] 
        [Range(1, 3)]
        public int PositiveQuirkMinimumMod;
        
        [BoxGroup("Simulation")] 
        [Range(1, 3)]
        public int PositiveQuirkMaximumMod;

        [BoxGroup("Simulation")] 
        [Range(0, 6)]
        public int PossibleNegativeQuirkCount;
        
        [BoxGroup("Simulation")] 
        [Range(-3, -1)]
        public int NegativeQuirkMinimumMod;
        
        [BoxGroup("Simulation")] 
        [Range(-3, -1)]
        public int NegativeQuirkMaximumMod;

        [BoxGroup("Simulation")] 
        [Range(1, 6)]
        public int MaxCapacity;
        
        [BoxGroup("Simulation")]
        public int Endurance;
        
        [BoxGroup("Simulation")]
        public int Days;

        [BoxGroup("Simulation")] 
        public int ModifierCount;

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
