using System.Collections.Generic;
using System.Linq;
using _Scripts.Gameplay;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.UI
{
    public class UILootEquipPopup : MonoBehaviour, IWorldInstanceController<Monster>
    {
        [SerializeField] private UILoot m_lootPrefab;
        [SerializeField] private Transform m_entryRoot;

        private Monster m_monster;
        private List<UILoot> m_entries = new ();

        private void Awake()
        {
            UpdateLootItems();

            ServiceLocator.Instance.LootManager.OnLootAdded += _ => { UpdateLootItems(); };
            ServiceLocator.Instance.LootManager.OnLootSold += _ => { UpdateLootItems(); };
            ServiceLocator.Instance.LootManager.OnLootEquipped += _ => { UpdateLootItems(); };
            ServiceLocator.Instance.LootManager.OnLootUnEquipped += _ => { UpdateLootItems(); };
        }

        public void SetInstance(Monster instance)
        {
            m_monster = instance;
            UpdateLootItems();
        }

        private void UpdateLootItems()
        {
            var ownedItems = ServiceLocator.Instance.LootManager.LootInstances.ToList();
            for (var i = 0; i < ownedItems.Count; i++)
            {
                var item = ownedItems[i];
                
                // Add an entry if we need to
                if (i >= m_entries.Count)
                {
                    var entry = Instantiate(m_lootPrefab, m_entryRoot);
                    m_entries.Add(entry);
                    
                    entry.Button.onClick.AddListener(() =>
                    {
                        if (entry.EquippedMonster != null && entry.EquippedMonster == m_monster)
                        {
                            ServiceLocator.Instance.LootManager.UnEquip(entry.Instance, m_monster);
                        }
                        else
                        {
                            ServiceLocator.Instance.LootManager.Equip(entry.Instance, m_monster);
                        }
            
                        // TODO: Better UX
                        // for now, close this popup
                        ServiceLocator.Instance.UIPopupManager.RequestClose();
                    });
                }

                m_entries[i].SetInstance(item);

                // Do not show items that are equipped from other monsters
                if (item.EquippedMonster != null && item.EquippedMonster != m_monster)
                {
                    m_entries[i].gameObject.SetActive(false);
                }
                else
                {
                    m_entries[i].gameObject.SetActive(true);
                }
            }
            
            // Handle leftovers
            if (m_entries.Count > ownedItems.Count)
            {
                for (int i = ownedItems.Count; i < m_entries.Count; i++)
                {
                    m_entries[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
