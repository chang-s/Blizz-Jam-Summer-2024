using Sirenix.OdinInspector;

namespace _Scripts.Schemas
{
    public abstract class Schema : SerializedScriptableObject
    {
        public virtual string GetTooltipText()
        {
            return string.Empty;
        }
    }
}