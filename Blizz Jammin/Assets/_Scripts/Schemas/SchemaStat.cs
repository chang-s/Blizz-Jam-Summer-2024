using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Stat")]
    public class SchemaStat: Schema
    {
        public enum Stat
        {
            Attack,
            Endurance,
            Luck,
            Symbiosis,
            Terror,    
        }
        
        /// <summary>
        /// The monster's name to be displayed in UI.
        /// </summary>
        [BoxGroup("Visuals")]
        public string Name;
        
        /// <summary>
        /// The sprite used when shown in the world/UI.
        /// </summary>
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite Sprite;

        /// <summary>
        /// For referencing directly with code.
        /// </summary>
        public Stat Type;
    }
}
