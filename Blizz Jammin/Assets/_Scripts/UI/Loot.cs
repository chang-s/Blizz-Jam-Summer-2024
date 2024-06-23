using System.Collections.Generic;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    // TODO: Separate the UI side of this into UILoot : IWorldInstanceController<Loot>
    // this is acting as the instance AND the inventory UI right now. not great
    public class Loot : WorldInstance, ISchemaController<SchemaLoot>
    {
        public enum LootState
        {
            NotOwned,
            Owned,
            Equipped
        }
        
        public SchemaLoot Data { get; private set; }
        public Monster EquippedMonster { get; private set; }
        public Button Button => m_button;

        public LootState State { get; private set; } = LootState.NotOwned;
        public bool IsNew { get; private set; } = true;

        [SerializeField] private Dictionary<Loot.LootState, GameObject> m_states = new();

        [SerializeField] private Button m_button;
        
        [SerializeField] private Image m_icon;
        [SerializeField] [CanBeNull] private Image m_equippedMonsterIcon;
        [SerializeField] [CanBeNull] private GameObject m_badge;
        [SerializeField] [CanBeNull] private TMP_Text m_name;
        [SerializeField] [CanBeNull] private TMP_Text m_description;
        
        private void UpdateStateVisuals()
        {
            foreach (var (state, group) in m_states)
            {
                group.SetActive(state == State);
            }
            
            switch (State)
            {
                case LootState.Equipped:
                    m_states[LootState.Owned].SetActive(true);
                    break;
            }
        }

        public void Grant()
        {
            if (State != LootState.NotOwned)
            {
                return;
            }

            IsNew = true;
            m_badge?.SetActive(true);
            
            State = LootState.Owned;
            UpdateStateVisuals();
        }
        
        public void SetData(SchemaLoot data)
        {
            Data = data;
            
            m_icon.sprite = data.Icon;
            m_name?.SetText(data.Name);
            m_description?.SetText(data.Description);
        }

        public void MarkSeen()
        {
            IsNew = false;
            m_badge?.SetActive(false);
        }
        
        // TODO: THIS SHOULD BE HANDLED BY MANAGER
        // response via the event should happen here
        public void Equip(Monster monster)
        {
            if (State != LootState.Owned)
            {
                return;
            }

            if (m_equippedMonsterIcon != null)
            {
                m_equippedMonsterIcon.sprite = monster.Data.Sprite;
            }
            
            monster.EquippedLoot.Add(this);
            EquippedMonster = monster;
            State = LootState.Equipped;
            UpdateStateVisuals();
        }

        // TODO: THIS SHOULD BE HANDLED BY MANAGER
        // response via the event should happen here
        public void UnEquip()
        {
            if (State != LootState.Equipped)
            {
                return;
            }
            
            if (m_equippedMonsterIcon != null)
            {
                m_equippedMonsterIcon.sprite = null;
            }
            
            EquippedMonster.EquippedLoot.Remove(this);
            EquippedMonster = null;
            State = LootState.Owned;
            UpdateStateVisuals();
        }

    }
}