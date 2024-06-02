using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Loot")]
    public class SchemaLoot : Schema
    {
        /// <summary>
        /// The loot's name to be displayed in UI.
        /// </summary>
        [BoxGroup("Visuals")]
        public string Name;

        /// <summary>
        /// The loot's description to be displayed in UI.
        /// </summary>
        [BoxGroup("Visuals")] 
        public string Description;
        
        /// <summary>
        /// The sprite used when shown in the world/UI.
        /// </summary>
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite Sprite;
    }

    
}
