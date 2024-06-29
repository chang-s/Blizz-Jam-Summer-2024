using _Scripts.Gameplay;
using _Scripts.Schemas;
using DG.Tweening;
using System.Threading;
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
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScaleY(.75f, .05f));
            sequence.Insert(.05f, transform.DOScaleX(1.1f, .2f));
            sequence.Insert(.05f, transform.DOScaleZ(1.1f, .2f));
            sequence.Append(transform.DOScale(1f, .1f));

            // Do the sound
            ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.ButtonClick);
            
            ServiceLocator.Instance.UIPopupManager.RequestPopup(PopupType);
        }
    }
}
