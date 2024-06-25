using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/LootInstances")]
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
        /// How many stars this item has.
        /// </summary>
        [BoxGroup("Visuals")] 
        [MinValue(1)]
        public int StarQuality;
        
        /// <summary>
        /// The sprite used when shown in the world/UI.
        /// </summary>
        [BoxGroup("Visuals")]
        [PreviewField(100)]
        public Sprite Icon;

        /// <summary>
        /// The amount of Infamy gained when sold.
        /// </summary>
        [BoxGroup("Behavior")] 
        public int SellValue;
        
        /// <summary>
        /// The stats this loot provides.
        /// </summary>
        [BoxGroup("Behavior")] 
        public StatModifier[] Modifiers;
    }
}
