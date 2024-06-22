using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UILootDetails :  SerializedMonoBehaviour, IWorldInstanceController<Loot>
    {
        [SerializeField] private TMP_Text m_nameLabel;
        [SerializeField] private TMP_Text m_descriptionLabel;
        [SerializeField] private TMP_Text m_valueLabel;
        [SerializeField] private Image m_icon;
        [SerializeField] private Transform m_inventoryRoot;
        [SerializeField] private Button m_sellButton;

        private Loot m_shownInstance;
        
        private void Awake()
        {
            ServiceLocator.Instance.LootManager.OnLootAdded += OnLootAdded;
            ServiceLocator.Instance.LootManager.OnLootSold += OnLootSold;
            
            m_sellButton.onClick.AddListener(SellItem);
        }

        private void SellItem()
        {
            if (m_shownInstance == null)
            {
                return;
            }

            ServiceLocator.Instance.LootManager.SellLoot(m_shownInstance);
            m_shownInstance = null;
            
            // TODO: Show something else, and cover our asses
            // for now, just close the popup...
            ServiceLocator.Instance.UIPopupManager.RequestClose();
        }

        private void OnLootAdded(Loot instance)
        {
            instance.transform.SetParent(m_inventoryRoot);
            
            instance.Button.onClick.AddListener(() =>
            {
                SetInstance(instance);
            });
        }
        
        private void OnLootSold(Loot instance)
        {
            instance.Button.onClick.RemoveAllListeners();
            Destroy(instance.gameObject);
        }

        public void SetInstance(Loot instance)
        {
            m_icon.sprite = instance.Data.Icon;
            m_nameLabel.SetText(instance.Data.Name);
            m_descriptionLabel.SetText(instance.Data.Description);
            m_valueLabel.SetText(instance.Data.SellValue.ToString());
        }
    }
}
