using System;
using System.Collections.Generic;
using _Scripts.Schemas;
using _Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class LootManager : SerializedMonoBehaviour
    {
        [SerializeField] private Transform m_lootWaitingRoom;
        [SerializeField] private Loot m_lootPrefab;
        
        [HideInInspector] public Action<Loot> OnLootAdded;
        [HideInInspector] public Action<Loot> OnLootSold;
        [HideInInspector] public Action<Loot> OnLootEquipped;
        [HideInInspector] public Action<Loot> OnLootUnEquipped;

        public IReadOnlyCollection<Loot> Loot => m_loot;

        private List<Loot> m_loot = new List<Loot>();

        public void GrantLoot(SchemaLoot schema)
        {
            Loot loot = Instantiate(m_lootPrefab, m_lootWaitingRoom);
            loot.SetData(schema);
            loot.Grant();

            m_loot.Add(loot);
            OnLootAdded?.Invoke(loot);
        }
        
        public void SellLoot(Loot loot)
        {
            ServiceLocator.Instance.DeltaInfamy(loot.Data.SellValue);

            // Make sure to remove it from any monster that may have it on them
            if (loot.EquippedMonster != null)
            {
                UnEquip(loot, loot.EquippedMonster);
            }
            
            m_loot.Remove(loot);
            OnLootSold?.Invoke(loot);
        }

        public void Equip(Loot loot, Monster monster)
        {
            if (loot == null || monster == null)
            {
                return;
            }

            // Only 3 slots
            if (monster.EquippedLoot.Count >= 3)
            {
                return;
            }

            // Already in use
            if (loot.EquippedMonster != null)
            {
                return;
            }

            loot.Equip(monster);

            OnLootEquipped?.Invoke(loot);
        }
        
        public void UnEquip(Loot loot, Monster monster)
        {
            if (loot == null || monster == null)
            {
                return;
            }

            if (loot.EquippedMonster == null)
            {
                return;
            }

            if (!monster.EquippedLoot.Contains(loot))
            {
                return;
            }
            
            loot.UnEquip();
            OnLootUnEquipped?.Invoke(loot);
        }
    }
}
