using _Scripts.Gameplay;
using _Scripts.Schemas;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIPopupRequester : MonoBehaviour
    {
        public Button Button;
        public SchemaPopup.PopupType PopupType;

        private void Start()
        {
            Button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            // Do the sound
            ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.ButtonClick);
            
            ServiceLocator.Instance.UIPopupManager.RequestPopup(PopupType);
        }
    }
}
