using _Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIPopupRequester : MonoBehaviour
    {
        public Button Button;
        public UIPopupManager.PopupType PopupType;

        private void Start()
        {
            Button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            ServiceLocator.Instance.UIPopupManager.RequestPopup(PopupType);
        }
    }
}
