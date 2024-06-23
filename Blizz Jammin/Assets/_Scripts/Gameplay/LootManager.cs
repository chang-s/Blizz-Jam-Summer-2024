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

        public IReadOnlyCollection<Loot> Loot => m_loot;

        private List<Loot> m_loot = new List<Loot>();
        
        // TODO:
        public void GrantLoot(SchemaLoot schema)
        {
            Loot loot = Instantiate(m_lootPrefab, m_lootWaitingRoom);
            loot.SetData(schema);
            loot.Grant();

            m_loot.Add(loot);
            OnLootAdded?.Invoke(loot);
        }

        // TODO:
        public void SellLoot(Loot loot)
        {
            ServiceLocator.Instance.DeltaInfamy(loot.Data.SellValue);
            
            m_loot.Remove(loot);
            OnLootSold?.Invoke(loot);
        }
    }
}
