using UnityEngine;

namespace _Scripts.UI.Tooltip
{
    public class UITooltipRequesterStatic : UITooltipRequester
    {
        [SerializeField] private string m_text;
        
        public override string GetText()
        {
            return m_text;
        }
    }
}