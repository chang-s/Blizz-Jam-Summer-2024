using _Scripts.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI.Tooltip
{
    public abstract class UITooltipRequester : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public abstract string GetText();

        public void OnPointerEnter(PointerEventData eventData)
        {
            ServiceLocator.Instance.UIPopupManager.Tooltip.Show(GetText());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ServiceLocator.Instance.UIPopupManager.Tooltip.Hide();
        }
    }
}
