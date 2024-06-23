using System.Collections.Generic;
using System.Linq;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIPopupManager : SerializedMonoBehaviour
    {
        [SerializeField] private Button m_sharedBG;
        [SerializeField] private Transform m_root;

        /// <summary>
        /// All the popup instances for the game.
        /// </summary>
        private Dictionary<SchemaPopup.PopupType, UIPopup> m_instances = new Dictionary<SchemaPopup.PopupType, UIPopup>();
        
        /// <summary>
        /// The queue of popups to be shown.
        /// </summary>
        private List<SchemaPopup.PopupType> m_queue = new List<SchemaPopup.PopupType>();

        /// <summary>
        /// The instance of the current popup type.
        /// </summary>
        private Stack<UIPopup> m_popupStack = new Stack<UIPopup>();

        private void Awake()
        {
            foreach (var schemaPopup in ServiceLocator.Instance.AllPopups)
            {
                UIPopup instance = Instantiate(schemaPopup.Prefab, m_root);
                instance.SetData(schemaPopup);
                
                instance.Hide();
                m_instances.Add(schemaPopup.Type, instance);
            }

            m_sharedBG.onClick.AddListener(OnBGClicked);
        }

        private void OnBGClicked()
        {
            if (m_popupStack.Count > 0 && m_popupStack.Peek().Schema.DismissOnBGTap)
            {
                RequestClose();
            }
        }

        // TODO: Type this better, so we don't incur boxing
        public UIPopup GetPopup(SchemaPopup.PopupType type)
        {
            if (!m_instances.ContainsKey(type))
            {
                return null;
            }

            return m_instances[type];
        }
        
        public void RequestPopup(SchemaPopup.PopupType type)
        {
            var popup = m_instances[type];
            if (popup.Schema.BypassQueue)
            {
                popup.Show();
                popup.transform.SetAsLastSibling();
                m_popupStack.Push(popup);
                return;
            }
            
            m_queue.Add(type);
            ProcessQueue();
        }
        
        public void RequestClose()
        {
            if (m_popupStack.Count <= 0)
            {
                return;
            }

            // Hide
            m_popupStack.Pop().Hide();

            // Try to go on
            ProcessQueue();
        }

        public bool HasActivePopup()
        {
            return m_popupStack.Count > 0;
        }
        
        private void ProcessQueue()
        {
            if (HasActivePopup())
            {
                return;
            }

            if (m_queue.Count == 0)
            {
                m_sharedBG.gameObject.SetActive(false);
                return;
            }

            SchemaPopup.PopupType popupType = m_queue[0];
            m_queue.RemoveAt(0);
        
            // Show
            m_instances[popupType].Show();
            m_popupStack.Push(m_instances[popupType]);
            m_sharedBG.gameObject.SetActive(true);
        }
    }
}
