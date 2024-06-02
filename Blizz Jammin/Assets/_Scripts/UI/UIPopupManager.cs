using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIPopupManager : MonoBehaviour
    {
        [Serializable]
        public struct PopupConfig
        {
            public PopupType m_type;
            public UIPopup m_prefab;
        }
        
        public enum PopupType
        {
            MonsterDetails,
        }
        
        [SerializeField] private GameObject m_sharedBG;
        [SerializeField] private Transform m_root;
        [SerializeField] private List<PopupConfig> m_configs = new List<PopupConfig>();
        
        // For some reason, Unity doesn't want to serialize this dictionary. PopupConfig is a struct intermediate.
        private Dictionary<PopupType, UIPopup> m_instances = new Dictionary<PopupType, UIPopup>();
        
        /// <summary>
        /// The queue of popups to be shown.
        /// </summary>
        private List<PopupType> m_queue = new List<PopupType>();

        /// <summary>
        /// The instance of the current popup type.
        /// </summary>
        private UIPopup m_currentPopup;

        private void Start()
        {
            foreach (var config in m_configs)
            {
                UIPopup instance = Instantiate(config.m_prefab, m_root);
                instance.Hide();
                m_instances.Add(config.m_type, instance);
            }
        }

        public void RequestPopup(PopupType type)
        {
            m_queue.Add(type);
            ProcessQueue();
        }

        public void RequestClose()
        {
            if (m_currentPopup == null)
            {
                return;
            }

            // Hide
            m_currentPopup.Hide();
            m_currentPopup = null;
        
            // Try to go on
            ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (m_currentPopup != null)
            {
                return;
            }

            if (m_queue.Count == 0)
            {
                m_sharedBG.SetActive(false);
                return;
            }

            PopupType popupType = m_queue[0];
            m_queue.RemoveAt(0);
        
            // Show
            m_currentPopup = m_instances[popupType];
            m_currentPopup.Show();
            m_sharedBG.SetActive(true);
        }
    }
}
