using System;
using System.Collections.Generic;
using _Scripts.Gameplay.Instances;
using _Scripts.Schemas;
using _Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        /// <summary>
        /// A tracker for showing if its newly unlocked.
        /// </summary>
        public bool IsNew { get; private set; } = false;

        /// <summary>
        /// TEMP
        /// </summary>
        public List<InstanceLoot> EquippedLoot = new List<InstanceLoot>();

        /// <summary>
        /// Read only version of the quirks.
        /// </summary>
        public IReadOnlyCollection<SchemaQuirk> Quirks => m_quirks;

        public Image m_ImageRenderer;

        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private TextMeshPro m_nameLabel;
        [SerializeField] private Dictionary<MonsterStatus, GameObject> m_states;

        private Dictionary<SchemaStat.Stat, int> m_stats = new();

        private HashSet<SchemaQuirk> m_quirks = new HashSet<SchemaQuirk>();

        private SchemaGameSettings m_gameSettings;

        public void SetData(SchemaMonster data)
        {
            Data = data;

            m_stats.Clear();
            foreach (var (schemaStat, amount) in Data.Stats)
            {
                m_stats.Add(schemaStat.Type, amount);
            }

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            m_nameLabel.SetText(Data.Name);
            m_spriteRenderer.transform.localScale = Data.Scale;
            m_ImageRenderer.sprite = Data.Sprite;
            m_spriteRenderer.sprite = Data.Sprite;
            
            foreach (var (state, group) in m_states)
            {
                group.SetActive(state == Status);
            }
        }

        /// <summary>
        /// TODO: CLEANUP
        /// DO NOT call this outside of MonsterManager!
        /// </summary>
        public void Unlock()
        {
            if (Status != MonsterStatus.Locked)
            {
                return;
            }

            // When unlocking a monster, roll its quirks
            for (int i = 0; i < Data.InitialQuirkCount; i++)
            {
                RollQuirk();
            }

            IsNew = true;
            Status = MonsterStatus.Purchasable;
            UpdateVisuals();
        }

        private void RollQuirk()
        {
            var allQuirks = ServiceLocator.Instance.AllQuirks;
            if (allQuirks.Length <= Quirks.Count)
            {
                return;
            }
            
            bool addedQuirk = false;
            while (!addedQuirk)
            {
                addedQuirk = m_quirks.Add(allQuirks[Random.Range(0, allQuirks.Length)]);
            }
        }

        /// <summary>
        /// TODO: CLEANUP
        /// DO NOT call this outside of MonsterManager!
        /// </summary>
        public void Recruit()
        {
            if (Status != MonsterStatus.Purchasable)
            {
                return;
            }

            Status = MonsterStatus.Ready;
            UpdateVisuals();
        }

        public void MarkSeen()
        {
            IsNew = false;
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

        public int GetStatValue(SchemaStat.Stat stat)
        {
            int valueScaledToLevel = (int) (m_stats[stat] * Math.Pow(m_gameSettings.StatLevelExponent, Level - 1));
            var bonusTuple = GetLootStatBonuses(stat);
            
            int flat = bonusTuple.Item1;
            int totalBeforeMult = valueScaledToLevel + flat;

            float mult = bonusTuple.Item2;
            int statValueRaw = totalBeforeMult + (int) (mult * totalBeforeMult);
            
            // Do not allow stats to go below 0
            return Math.Max(0, statValueRaw);
        }

        private (int, float) GetLootStatBonuses(SchemaStat.Stat stat)
        {
            // Get all the buffs from items on you or your team
            Monster[] monsters = new Monster[1];
            monsters[0] = this;

            if (CurrentMission != null)
            {
                monsters = ServiceLocator.Instance.MonsterManager.GetParty(CurrentMission);
            }
            
            int flat = 0;
            float mult = 0f;
            for (var i = 0; i < monsters.Length; i++)
            {
                var monster = monsters[i];
                if (monster == null)
                {
                    continue;
                }
                
                foreach (var loot in monster.EquippedLoot)
                {
                    foreach (var dataModifier in loot.Data.Modifiers)
                    {
                        // Skip this mod, its not of our stat
                        if (dataModifier.Stat.Type != stat)
                        {
                            continue;
                        }
                        
                        // The wearer of the loot does not pass requirements
                        if (!dataModifier.Passes(loot))
                        {
                            continue;
                        }

                        // The loot is not owned by us and its not an aura, so we do not benefit
                        if (monster != this && !dataModifier.Aura)
                        {
                            continue;
                        }
                        
                        // We benefit from this loot!
                        flat += dataModifier.FlatAmount;
                        mult += dataModifier.MultAmount;
                    }
                }   
            }

            return (flat, mult);
        }

        /// <summary>
        /// Adds the given amount of XP. Returns if a new level was reached.
        /// </summary>
        public bool AddXp(int amount)
        {
            Xp += amount;
            
            // At max level already
            int[] xpTables = m_gameSettings.XpForLevel;
            if (Level > xpTables.Length)
            {
                return false;
            }

            var xpForNextLevel = xpTables[Level - 1];
            if (Xp >= xpForNextLevel)
            {
                Level++;
                Xp -= xpForNextLevel;
                
                // Get a new quirk when you level!
                RollQuirk();
                
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
            
            ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.ButtonClick);

            // If we're locked, open the recruit popup instead of monster details
            if (Status == MonsterStatus.Locked || Status == MonsterStatus.Purchasable)
            {
                UIPopup rPopup = popupManager.GetPopup(SchemaPopup.PopupType.MonsterRecruit);
                UIMonsterRecruit monsterRecruit = rPopup.GetComponent<UIMonsterRecruit>();
                monsterRecruit.SetInstance(this);
                
                popupManager.RequestPopup(SchemaPopup.PopupType.MonsterRecruit);
                return;
            }
            
            UIPopup dPopup = popupManager.GetPopup(SchemaPopup.PopupType.MonsterDetails);
            UIMonsterDetails monsterDetails = dPopup.GetComponent<UIMonsterDetails>();
            monsterDetails.SetInstance(this);
            
            popupManager.RequestPopup(SchemaPopup.PopupType.MonsterDetails);
        }
    }
}
