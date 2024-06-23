using System;
using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UILootDetails :  SerializedMonoBehaviour, IWorldInstanceController<Loot>
    {
        private const string c_star = "<sprite name=\"Infamy\">";

        [SerializeField] private Material m_normalMaterial;
        [SerializeField] private Material m_secretMaterial;

        [SerializeField] private TMP_Text m_nameLabel;
        [SerializeField] private TMP_Text m_starLabel;
        [SerializeField] private TMP_Text m_descriptionLabel;
        [SerializeField] private TMP_Text m_valueLabel;
        [SerializeField] private Image m_icon;
        [SerializeField] private Transform m_inventoryRoot;
        [SerializeField] private Button m_sellButton;

        [SerializeField] private UILootModification[] m_lootMods;
        
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
            m_shownInstance = instance;
            
            m_icon.sprite = instance.Data.Icon;
            m_nameLabel.SetText(instance.Data.Name);

            string starString = "";
            for (int i = 0; i < instance.Data.StarQuality; i++)
            {
                starString += c_star;
            }
            m_starLabel.SetText(starString);
            
            m_descriptionLabel.SetText(instance.Data.Description);
            m_valueLabel.SetText(instance.Data.SellValue.ToString());
            
            // Handle the loot mods
            // TODO: Clean this shit up
            for (var i = 0; i < m_lootMods.Length; i++)
            {
                if (i >= instance.Data.Modifiers.Length)
                {
                    m_lootMods[i].Root.SetActive(false);
                    continue;
                }
                
                // We consider the required loot buffs "secret" because we wont show it until you activate it!
                var modifier = instance.Data.Modifiers[i];
                bool isSecretBuff = modifier.RequiredLoot != null && modifier.RequiredLoot.Length > 0;
                if (isSecretBuff && !modifier.Passes(instance))
                {
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

                result += modifier.Aura ? "to Party" : "to Self";

                m_lootMods[i].Icon.sprite = modifier.Stat.Sprite;
                m_lootMods[i].Label.SetText(result);
                m_lootMods[i].Label.color = isSecretBuff ? Color.white : Color.black;
                m_lootMods[i].Label.fontSharedMaterial = isSecretBuff ? m_secretMaterial : m_normalMaterial;
            }
        }
    }
}
