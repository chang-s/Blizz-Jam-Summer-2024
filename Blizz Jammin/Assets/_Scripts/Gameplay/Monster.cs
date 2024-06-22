using System;
using System.Collections.Generic;
using _Scripts.Schemas;
using _Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Monster : WorldInstance, ISchemaController<SchemaMonster>
    {
        public enum MonsterStatus
        {
            Locked,             // Cannot purchase yet
            Purchasable,        // Is in the shop
            Ready,              // Is bought and owned
            Busy                // Is owned, but is currently busy in combat or otherwise
        }
        
        /// <summary>
        /// The data that is being used for this instance.
        /// </summary>
        public SchemaMonster Data { get; private set; }

        /// <summary>
        /// The world status of this monster. Helps determine what they can and cannot do.
        /// </summary>
        public MonsterStatus Status { get; private set; } = MonsterStatus.Locked;

        /// <summary>
        /// TODO: Consider moving this tracking to MissionManager or MonsterManager
        /// The current mission this monster is in.
        /// </summary>
        public SchemaMission CurrentMission { get; private set; }
        
        /// <summary>
        /// The class of this monster if there is data set.
        /// </summary>
        public SchemaQuirk Class => Data != null ? Data.Class : null;

        /// <summary>
        /// The current level of the monster.
        /// </summary>
        public int Level { get; private set; } = 1;
        
        /// <summary>
        /// The amount of Xp in towards the next level.
        /// </summary>
        public int Xp { get; private set; } = 0;

        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private TextMeshPro m_nameLabel;
        [SerializeField] private Dictionary<MonsterStatus, GameObject> m_states;

        private Dictionary<SchemaStat.Stat, int> m_stats = new();
        private Dictionary<SchemaStat.Stat, int> m_flatStatBonus = new();
        private Dictionary<SchemaStat.Stat, float> m_mulltStatBonus = new();

        private List<SchemaQuirk> m_unlockedQuirks = new List<SchemaQuirk>();

        private SchemaGameSettings m_gameSettings;

        public void SetData(SchemaMonster data)
        {
            Data = data;

            m_stats.Clear();
            foreach (var (schemaStat, amount) in Data.Stats)
            {
                m_stats.Add(schemaStat.Type, amount);
                m_flatStatBonus.Add(schemaStat.Type, 0);
                m_mulltStatBonus.Add(schemaStat.Type, 0);
            }

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            m_nameLabel.SetText(Data.Name);
            m_spriteRenderer.transform.localScale = Data.Scale;
            m_spriteRenderer.sprite = Data.Sprite;
            
            foreach (var (state, group) in m_states)
            {
                group.SetActive(state == Status);
            }
        }

        public void Unlock()
        {
            if (Status != MonsterStatus.Locked)
            {
                return;
            }

            Status = MonsterStatus.Purchasable;
            UpdateVisuals();
        }

        public void Recruit()
        {
            if (Status != MonsterStatus.Purchasable)
            {
                return;
            }

            Status = MonsterStatus.Ready;
            UpdateVisuals();
        }
        
        public void BeginMission(SchemaMission mission)
        {
            // Can't start a mission if its not purchased
            if (Status < MonsterStatus.Ready)
            {
                return;
            }

            // Can't start a mission if you're busy
            if (Status == MonsterStatus.Busy)
            {
                return;
            }

            Status = MonsterStatus.Busy;
            CurrentMission = mission;
            UpdateVisuals();
        }

        public void EndMission(SchemaMission mission)
        {
            // You were not in a mission
            if (Status != MonsterStatus.Busy || CurrentMission == null)
            {
                return;
            }
            
            // Can't finish a mission you aren't on
            if (CurrentMission != mission)
            {
                Debug.LogWarning("Monster instructed to finish mission, but it was not the one it was on!");
                return;
            }

            Status = MonsterStatus.Ready;
            CurrentMission = null;
            UpdateVisuals();
        }
        
        public List<SchemaQuirk> GetUnlockedQuirks()
        {
            var allQuirks = ServiceLocator.Instance.AllQuirks;
            
            //Probably should rework this when level unlocks classes/quirks?
            m_unlockedQuirks.Clear();
            for (int i = 0; i < Level; ++i)
            {
                if (i < allQuirks.Length)
                {
                    m_unlockedQuirks.Add(allQuirks[i]);
                }
            }
            return m_unlockedQuirks;
        }

        public int GetStatValue(SchemaStat.Stat stat)
        {
            int valueScaledToLevel = (int) (m_stats[stat] * Math.Pow(m_gameSettings.StatLevelExponent, Level - 1));
            int flatBonus = m_flatStatBonus[stat];
            int totalBeforeMult = valueScaledToLevel + flatBonus;
            float multBonus = m_mulltStatBonus[stat];

            return totalBeforeMult + (int) (multBonus * totalBeforeMult);
        }

        public void DeltaFlatBonus(SchemaStat.Stat stat, int delta)
        {
            m_flatStatBonus[stat] += delta;
        }
        
        public void DeltaMultBonus(SchemaStat.Stat stat, float delta)
        {
            m_mulltStatBonus[stat] += delta;
        }

        /// <summary>
        /// Adds the given amount of XP. Returns if a new level was reached.
        /// </summary>
        public bool AddXp(int amount)
        {
            Xp += amount;
            
            // At max level already
            int[] xpTables = m_gameSettings.XpForLevel;
            if (Level >= xpTables.Length)
            {
                return false;
            }

            var xpForNextLevel = xpTables[Level];
            if (Xp > xpForNextLevel)
            {
                Level++;
                Xp -= xpForNextLevel;
                return true;
            }

            return false;
        }

        private void Awake()
        {
            m_gameSettings = ServiceLocator.Instance.GameSettings;
        }

        private void OnMouseDown()
        {
            // If we're already showing something, then disregard the click
            var popupManager = ServiceLocator.Instance.UIPopupManager;
            if (popupManager.HasActivePopup())
            {
                return;
            }
            
            // If we're locked, open the recruit popup instead of monster details
            // TODO: Make it load this specific monster
            if (Status == MonsterStatus.Locked || Status == MonsterStatus.Purchasable)
            {
                popupManager.RequestPopup(SchemaPopup.PopupType.MonsterRecruit);
                return;
            }
            
            UIPopup popup = popupManager.GetPopup(SchemaPopup.PopupType.MonsterDetails);
            UIMonsterDetails monsterDetails = popup.GetComponent<UIMonsterDetails>();
            monsterDetails.SetData(Data);
            monsterDetails.SetInstance(this);
            
            popupManager.RequestPopup(SchemaPopup.PopupType.MonsterDetails);
        }
    }
}
