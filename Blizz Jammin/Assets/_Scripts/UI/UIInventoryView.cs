using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Gameplay;
using _Scripts.Gameplay.Instances;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIInventoryView :  SerializedMonoBehaviour, IInstanceController<InstanceLoot>
    {
        [BoxGroup("Popup")]
        [SerializeField] private UIPopup m_popup;
        
        [BoxGroup("Inventory Area")]
        [SerializeField] private UILoot m_lootPrefab;
        [BoxGroup("Inventory Area")]
        [SerializeField] private Transform m_inventoryRoot;

        [BoxGroup("Item Inspected")] 
        [SerializeField] private UILoot m_uiLoot;
        [BoxGroup("Item Inspected")]
        [SerializeField] private Button m_sellButton;
        
        [BoxGroup("No Items")]
        [SerializeField] private GameObject m_noItemsPanel;
        
        [BoxGroup("Stats")]
        [SerializeField] private Material m_normalMaterial;
        [BoxGroup("Stats")]
        [SerializeField] private Material m_secretMaterial;
        [BoxGroup("Stats")]
        [SerializeField] private UILootModification[] m_lootMods;
        
        private InstanceLoot m_shownInstance;
        private List<UILoot> m_loot = new();
        
        private void Awake()
        {
            ServiceLocator.Instance.LootManager.OnLootAdded += OnLootAdded;
            ServiceLocator.Instance.LootManager.OnLootSold += OnLootSold;
            
            m_sellButton.onClick.AddListener(SellItem);
            m_noItemsPanel.SetActive(true);

            // When the popup is shown, we should mark the item as seen immediately and then
            // ensure we have the latest data for it
            // TODO: Ideally we do not need to do this
            m_popup.OnShow += () =>
            {
                if (m_shownInstance != null)
                {
                    ServiceLocator.Instance.LootManager.MarkSeen(m_shownInstance);
                    SetInstance(m_shownInstance);
                }
            };
        }

        public void SetInstance(InstanceLoot instance)
        {
            m_shownInstance = instance; ;
            m_uiLoot.SetInstance(instance);
            UpdateVisuals();
            
            // HACK: We are relying on Unity's selection state, but it gets cleared when you press "sell"
            // To remedy this, we should always select the instance that we are set to
            EventSystem.current.SetSelectedGameObject(GetUILoot(instance));
        }

        private GameObject GetUILoot(InstanceLoot instance)
        {
            var uiLoot = m_loot.Find(l => l.Instance == instance);
            return uiLoot != null ? uiLoot.gameObject : null;
        }

        private void UpdateVisuals()
        {
            // Handle the loot mods
            // TODO: Support more than 5 mod entries (m_lootMods == length 5 right now)
            // TODO: Clean this shit up
            for (var i = 0; i < m_lootMods.Length; i++)
            {
                if (i >= m_shownInstance.Data.Modifiers.Length)
                {
                    m_lootMods[i].Root.SetActive(false);
                    continue;
                }
                
                // We consider the required loot buffs "secret" because we wont show it until you activate it!
                var modifier = m_shownInstance.Data.Modifiers[i];
                bool isSecretBuff = (modifier.RequiredLoot != null && modifier.RequiredLoot.Length > 0) ||
                                    (modifier.RequiredQuirk != null && modifier.RequiredQuirk.Length > 0);
                if (isSecretBuff && !modifier.Passes(m_shownInstance))
                {
                    m_lootMods[i].Root.SetActive(false);
                    continue;
                }

                m_lootMods[i].Root.SetActive(true);
                
                string result = "";
                if (modifier.FlatAmount != 0)
                {
                    result = modifier.FlatAmount > 0 ? "+" : "-";
                    result += Math.Abs(modifier.FlatAmount);
                    result += " ";
                }
                else if (modifier.MultAmount != 0)
                {
                    result = modifier.MultAmount > 0 ? "+" : "-";
                    result += (Math.Abs(modifier.MultAmount) * 100).ToString();
                    result += "% ";
                }
                else
                {
                    // Special case for Wanted Poster
                    result += "+0";
                }

                result += modifier.Aura ? "to Party" : "to Self";

                m_lootMods[i].Icon.sprite = modifier.Stat.Sprite;
                m_lootMods[i].Label.SetText(result);
                m_lootMods[i].Label.color = isSecretBuff ? Color.white : Color.black;
                m_lootMods[i].Label.fontSharedMaterial = isSecretBuff ? m_secretMaterial : m_normalMaterial;
            }
        }
        
        private void SellItem()
        {
            if (m_shownInstance == null)
            {
                return;
            }
            
            ServiceLocator.Instance.LootManager.SellLoot(m_shownInstance);
        }

        private void OnLootAdded(InstanceLoot instance)
        {
            // When a loot is added, we can be sure there is at least 1, so we can turn this off
            m_noItemsPanel.SetActive(false);
            
            // Create a prefab to represent this item
            var loot = Instantiate(m_lootPrefab, m_inventoryRoot);
            loot.SetInstance(instance);
            m_loot.Add(loot);

            // First item added! Select it
            if (m_inventoryRoot.childCount == 1)
            {
                SetInstance(instance);
            }
            
            // Add a listener to the button to show that item's details
            loot.Button.onClick.AddListener(() =>
            {
                // Do the sound
                ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.ButtonClick);
                
                ServiceLocator.Instance.LootManager.MarkSeen(instance);
                SetInstance(instance);
            });
        }
        
        private void OnLootSold(InstanceLoot instance)
        {
            // Since the only time we can sell is when we view an item, we can assume we do not see it anymore
            m_shownInstance = null;

            // Find the loot item, if you can
            UILoot uiLoot = m_loot.Find(l => l.Instance == instance);
            int lootIndex = m_loot.FindIndex(l => l.Instance == instance);
            if (uiLoot == null)
            {
                return;
            }

            // Cleanup after this loot item
            uiLoot.Button.onClick.RemoveAllListeners();
            m_loot.Remove(uiLoot);
            DestroyImmediate(uiLoot.gameObject);
            
            // Select the item before it in the list
            var loot = ServiceLocator.Instance.LootManager.LootInstances.ToArray();
            if (loot.Length == 0)
            {
                m_noItemsPanel.SetActive(true);
                return;
            }

            if (loot.Length == 1)
            {
                ServiceLocator.Instance.LootManager.MarkSeen(loot[0]);
                SetInstance(loot[0]);
                return;
            }

            int newLootIndex = lootIndex;
            while (newLootIndex >= loot.Length)
            {
                newLootIndex--;
            }
            ServiceLocator.Instance.LootManager.MarkSeen(loot[newLootIndex]);
            SetInstance(loot[newLootIndex]);
        }
    }
}
