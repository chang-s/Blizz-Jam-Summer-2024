using System.Collections.Generic;
using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Mission")]
    public class SchemaMission : Schema
    {
        /// <summary>
        /// The monster's name to be displayed in UI.
        /// </summary>
        [BoxGroup("Visuals")]
        public string Name;
        
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite Icon;

        [BoxGroup("Requirements")] 
        public int MinCapacity;
        
        [BoxGroup("Requirements")] 
        public int MaxCapacity;
        
        [BoxGroup("Simulation")]
        public int Health;
        
        [BoxGroup("Simulation")]
        public int Days;
        
        [BoxGroup("Rewards")] 
        public SchemaLootTable LootTable;
        
        // TODO: Quirk System
        
        [BoxGroup("Rewards")] 
        public float Infamy;
        
        [BoxGroup("Rewards")] 
        public float Xp;
    }
}
