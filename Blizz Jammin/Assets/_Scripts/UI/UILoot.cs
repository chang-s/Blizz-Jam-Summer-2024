using System.Collections.Generic;
using _Scripts.Gameplay;
using _Scripts.Gameplay.Instances;
using _Scripts.Schemas;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UILoot : WorldInstance, IInstanceController<InstanceLoot>, ISchemaController<SchemaLoot>
    {
        private const string c_star = "<sprite name=\"Infamy\">";
        
        // Use this class to display a loot item anywhere in the game. Simply attach the script to a game object,
        // serialize anything you want to be updated when the loot is associated. Then, call SetInstance() with which
        // one you want to visualize.
        [SerializeField] [CanBeNull] private Dictionary<InstanceLoot.LootState, GameObject> m_states = new();
        [SerializeField] [CanBeNull] private Button m_button;
        [SerializeField] [CanBeNull] private Image m_icon;
        [SerializeField] [CanBeNull] private Image m_equippedMonsterIcon;
        [SerializeField] [CanBeNull] private TMP_Text m_name;
        [SerializeField] [CanBeNull] private TMP_Text m_description;
        [SerializeField] [CanBeNull] private TMP_Text m_valueLabel;
        [SerializeField] [CanBeNull] private TMP_Text m_starLabel;
        
        public InstanceLoot Instance { get; private set; }
        public SchemaLoot Data => Instance != null ? Instance.Data : m_instancelessData;
        public Button Button => m_button;
        public Monster EquippedMonster => Instance != null ? Instance.EquippedMonster : null;

        private SchemaLoot m_instancelessData;

        private void Awake()
        {
            ServiceLocator.Instance.LootManager.OnLootEquipped += loot =>
            {
                if (loot != Instance)
                {
                    return;
                }

                UpdateStateVisuals();
            };
            
            ServiceLocator.Instance.LootManager.OnLootUnEquipped += loot =>
            {
                if (loot != Instance)
                {
                    return;
                }

                UpdateStateVisuals();
            };
        }

        public void SetInstance(InstanceLoot instance)
        {
            Instance = instance;
            UpdateStateVisuals();
        }
        
        private void UpdateStateVisuals()
        {
            if (m_icon != null)
            {
                m_icon.sprite = Data.Icon;
            }

            if (m_equippedMonsterIcon != null)
            {
                m_equippedMonsterIcon.sprite = EquippedMonster != null ? EquippedMonster.Data.Sprite : null;
            }
            
            m_name?.SetText(Data.Name);
            m_description?.SetText(Data.Description);
            m_valueLabel?.SetText(Data.SellValue.ToString());
            
            string starString = "";
            for (int i = 0; i < Data.StarQuality; i++)
            {
                starString += c_star;
            }
            m_starLabel?.SetText(starString);
            
            if (m_states != null && Instance != null)
            {
                foreach (var (state, group) in m_states)
                {
                    group.SetActive(state == Instance.State);
                }
            
                switch (Instance.State)
                {
                    case InstanceLoot.LootState.OwnedNew:
                    case InstanceLoot.LootState.Equipped:
                        if (m_states.ContainsKey(InstanceLoot.LootState.Owned))
                        {
                            m_states[InstanceLoot.LootState.Owned].SetActive(true);
                        }
                        break;
                }
            }
        }

        // TODO: CLEAN THIS UP
        // We have the MissionDetails which is showing "not real" items. AKA they are not created/granted yet,
        // but we still need to display them. Use THIS method instead of SetInstance in those cases
        public void SetData(SchemaLoot data)
        {
            m_instancelessData = data;
            UpdateStateVisuals();
        }
    }
}
