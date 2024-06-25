using System;
using System.Collections.Generic;
using _Scripts.Gameplay.Instances;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class LootManager : SerializedMonoBehaviour
    {
        [HideInInspector] public Action<InstanceLoot> OnLootAdded;
        [HideInInspector] public Action<InstanceLoot> OnLootSold;
        [HideInInspector] public Action<InstanceLoot> OnLootEquipped;
        [HideInInspector] public Action<InstanceLoot> OnLootUnEquipped;
        [HideInInspector] public Action<InstanceLoot> OnLootSeen;

        public IReadOnlyCollection<InstanceLoot> LootInstances => m_lootInstances;
        private List<InstanceLoot> m_lootInstances = new List<InstanceLoot>();

        public void GrantLoot(SchemaLoot schema)
        {
            InstanceLoot instance = new InstanceLoot(schema);
            m_lootInstances.Add(instance);
            OnLootAdded?.Invoke(instance);
        }
        
        public void SellLoot(InstanceLoot instance)
        {
            // Gain the infamy value of the loot
            ServiceLocator.Instance.DeltaInfamy(instance.Data.SellValue);

            // Make sure to remove it from any monster that may have it on them
            if (instance.EquippedMonster != null)
            {
                UnEquip(instance, instance.EquippedMonster);
            }
            
            m_lootInstances.Remove(instance);
            OnLootSold?.Invoke(instance);
        }

        public void MarkSeen(InstanceLoot loot)
        {
            loot.MarkSeen();
            OnLootSeen?.Invoke(loot);
        }

        public void Equip(InstanceLoot loot, Monster monster)
        {
            if (loot == null || monster == null)
            {
                return;
            }

            if (!loot.Equip(monster))
            {
                return;
            }
            
            OnLootEquipped?.Invoke(loot);
        }
        
        public void UnEquip(InstanceLoot loot, Monster monster)
        {
            // Invalid request
            if (loot == null || monster == null)
            {
                return;
            }

            if (!loot.UnEquip(monster))
            {
                return;
            }
            
            OnLootUnEquipped?.Invoke(loot);
        }
    }
}
