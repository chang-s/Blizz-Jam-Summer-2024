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

        [BoxGroup("Behavior")]
        public int Health;
        
        [BoxGroup("Behavior")]
        public int Days;

        [BoxGroup("Behavior")] 
        public int MinCapacity;
        
        [BoxGroup("Behavior")] 
        public int MaxCapacity;
        
        [BoxGroup("Behavior")] 
        public float InfamyScalar;
        
        [BoxGroup("Behavior")] 
        public float XpScalar;

        // TODO: Loot Table
    }
}
