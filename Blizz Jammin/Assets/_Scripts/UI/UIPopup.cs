using System;
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

        public Action OnShow;
        public Action OnHide;

        public bool Showing { get; private set; }
        
        public void Show()
        {
            Showing = true;
            m_canvasGroup.alpha = 1.0f;
            m_canvasGroup.blocksRaycasts = true;
            OnShow?.Invoke();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_content);
        }

        public void Hide()
        {

            Showing = false;
            m_canvasGroup.alpha = 0.0f;
            m_canvasGroup.blocksRaycasts = false;
            OnHide?.Invoke();
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
