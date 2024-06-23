using System.Collections.Generic;
using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UILootEquipEntry : SerializedMonoBehaviour, IWorldInstanceController<Loot>, IWorldInstanceController<Monster>
    {
        [SerializeField] private Dictionary<Loot.LootState, GameObject> m_states = new();
        [SerializeField] private Button m_button;
        [SerializeField] private Image m_lootIcon;
            
        private Loot m_loot;
        private Monster m_monster;

        private void Awake()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (m_loot == null || m_monster == null)
            {
                return;
            }

            if (m_loot.EquippedMonster != null && m_loot.EquippedMonster == m_monster)
            {
                ServiceLocator.Instance.LootManager.UnEquip(m_loot, m_monster);
            }
            else
            {
                ServiceLocator.Instance.LootManager.Equip(m_loot, m_monster);
            }
            
            // TODO: Better UX
            // for now, close this popup
            ServiceLocator.Instance.UIPopupManager.RequestClose();
        }

        public void SetInstance(Loot instance)
        {
            m_loot = instance;

            m_lootIcon.sprite = instance.Data.Icon;
            
            UpdateState();
        }

        public void SetInstance(Monster instance)
        {
            m_monster = instance;
            UpdateState();
        }
        
        private void UpdateState()
        {
            foreach (var (state, group) in m_states)
            {
                group.SetActive(state == m_loot.State);
            }
        }
    }
}
