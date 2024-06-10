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

        [BoxGroup("Requirements")] 
        [Range(1, 6)]
        public int MaxCapacity;
        
        [BoxGroup("Simulation")]
        public int Endurance;
        
        [BoxGroup("Simulation")]
        public int Days;
        
        [BoxGroup("Rewards")] 
        public SchemaLootTable LootTable;
        
        // TODO: Quirk/Type System
        
        [BoxGroup("Rewards")] 
        public float Infamy;
        
        [BoxGroup("Rewards")] 
        public float Xp;
    }
}
