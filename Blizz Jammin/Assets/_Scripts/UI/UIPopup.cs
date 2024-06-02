using _Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private RectTransform m_content;
        [SerializeField] private Button[] m_closeButtons;

        public void Show()
        {
            m_canvasGroup.alpha = 1.0f;
            m_canvasGroup.blocksRaycasts = true;
        
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_content);
        }

        public void Hide()
        {
            m_canvasGroup.alpha = 0.0f;
            m_canvasGroup.blocksRaycasts = false;
        }

        private void Start()
        {
            foreach (var button in m_closeButtons)
            {
                button.onClick.AddListener(OnCloseButtonPressed);
            }
        }
    
        private void OnCloseButtonPressed()
        {
            ServiceLocator.Instance.UIPopupManager.RequestClose();
        }
    }
}
