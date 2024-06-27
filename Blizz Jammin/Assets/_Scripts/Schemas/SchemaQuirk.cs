using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Schemas
{
    [CreateAssetMenu(menuName = "Schema/Quirk")]
    public class SchemaQuirk : Schema
    {
        [BoxGroup("Visuals")] 
        public string Name;
        
        [BoxGroup("Visuals")] 
        public string TooltipText;
        
        [BoxGroup("Visuals")] 
        public Sprite Icon;
        
        public override string GetTooltipText()
        {
            return TooltipText;
        }
    }
}
