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
    }

    
}
