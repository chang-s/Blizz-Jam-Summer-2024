using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIPopupManager : SerializedMonoBehaviour
    {
        public enum PopupType
        {
            MonsterDetails,
            MissionDetails,
            MissionResults,
        }
        
        [SerializeField] private GameObject m_sharedBG;
        [SerializeField] private Transform m_root;
        [SerializeField] private Dictionary<PopupType, UIPopup> m_configs;
        
        /// <summary>
        /// All the popup instances for the game.
        /// </summary>
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
            foreach (var (type, prefab) in m_configs)
            {
                UIPopup instance = Instantiate(prefab, m_root);
                instance.Hide();
                m_instances.Add(type, instance);
            }
        }

        // TODO: Type this better, so we don't incur boxing
        public UIPopup GetPopup(PopupType type)
        {
            if (!m_instances.ContainsKey(type))
            {
                return null;
            }

            return m_instances[type];
        }
        
        public void RequestPopup(PopupType type)
        {
            m_queue.Add(type);
            ProcessQueue();
        }

        // TODO: Should this require a reference to a specific popup? Currently it will close the top level which
        // may not always be what we want to do
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

        public bool HasActivePopup()
        {
            return m_currentPopup != null;
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
