using System;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIPopup : MonoBehaviour, ISchemaController<SchemaPopup>
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private RectTransform m_content;
        [SerializeField] private Button[] m_closeButtons;
        [SerializeField] private float m_animationTime;
        [SerializeField] private bool m_doEnterAnimation;
        [SerializeField] private bool m_doFadeAnimation;
        

        public Action OnShow;
        public Action OnHide;
        
        public SchemaPopup Schema { get; private set; }

        public bool Showing { get; private set; }
        
        public void Show()
        {
            m_canvasGroup.alpha = 1.0f;

            if(!Showing)
                EnterAnimations();

            Showing = true;
            m_canvasGroup.blocksRaycasts = true;
            OnShow?.Invoke();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_content);
        }

        public void Hide()
        {
            m_canvasGroup.alpha = 0.0f;

            if (Showing)
                ExitAnimations();

            Showing = false;
            m_canvasGroup.blocksRaycasts = false;
            OnHide?.Invoke();
        }

        private void EnterAnimations()
        {
            if (m_doEnterAnimation)
            {
                m_content.transform.localPosition = new Vector3(0f, -2000f, 0f);
                m_content.DOAnchorPos(new Vector2(0f, 0f), m_animationTime, false).SetEase(Ease.OutElastic);
            }
            if(m_doFadeAnimation)
            {
                m_canvasGroup.alpha = 0f;
                m_canvasGroup.DOFade(1f, m_animationTime);
            }
        }

        private void ExitAnimations()
        {
            if(m_doEnterAnimation)
            {
                m_content.transform.localPosition = Vector3.zero;
                m_content.DOAnchorPos(new Vector2(0f, -2000f), m_animationTime).SetEase(Ease.InOutQuint);
            }
            if (m_doFadeAnimation)
            {
                m_canvasGroup.alpha = 1f;
                m_canvasGroup.DOFade(1f, m_animationTime);
            }
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

        public void SetData(SchemaPopup data)
        {
            Schema = data;
        }
    }
}
