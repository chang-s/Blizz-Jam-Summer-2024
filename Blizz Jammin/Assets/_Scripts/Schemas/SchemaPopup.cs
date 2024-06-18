using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Popup")]
    public class SchemaPopup : Schema
    {
        /// <summary>
        /// This is so that code can easily reference any popup via an enum instead of having a reference
        /// to the Schema asset that drives it.
        /// </summary>
        public enum PopupType
        {
            MonsterDetails,
            MissionDetails,
            MissionResults,
            
            // Not used yet
            MonsterRecruit,
            Inventory,
            ItemDetails,
        }

        /// <summary>
        /// The game object prefab for this popup.
        /// </summary>
        public UIPopup Prefab;
        
        /// <summary>
        /// The type of this popup entry. DO NOT REUSE these. 1 popup == 1 popup type.
        /// </summary>
        public PopupType Type;
    }
}
