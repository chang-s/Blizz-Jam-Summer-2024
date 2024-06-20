using System;
using System.Collections.Generic;
using _Scripts.Schemas;
using _Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Monster : MonoBehaviour, ISchemaController<SchemaMonster>
    {
        public SchemaMonster Data { get; private set; }

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

        private Dictionary<SchemaStat.Stat, int> m_stats = new();
        private Dictionary<SchemaStat.Stat, int> m_flatStatBonus = new();
        private Dictionary<SchemaStat.Stat, float> m_mulltStatBonus = new();

        private List<SchemaQuirk> m_unlockedQuirks = new List<SchemaQuirk>();
        private SchemaQuirk m_class;

        private SchemaGameSettings m_gameSettings;

        public void SetData(SchemaMonster data)
        {
            Data = data;
            
            m_nameLabel.SetText(data.Name);
            m_spriteRenderer.transform.localScale = data.Scale;
            m_spriteRenderer.sprite = data.Sprite;
            
            m_stats.Clear();
            foreach (var (schemaStat, amount) in Data.Stats)
            {
                m_stats.Add(schemaStat.Type, amount);
                m_flatStatBonus.Add(schemaStat.Type, 0);
                m_mulltStatBonus.Add(schemaStat.Type, 0);
            }
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

        public SchemaQuirk GetClass()
        {
            if (Data == null)
            {
                return null;
            }

            return Data.Class;
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
            
            UIPopup popup = popupManager.GetPopup(SchemaPopup.PopupType.MonsterDetails);
            UIMonsterDetails monsterDetails = popup.GetComponent<UIMonsterDetails>();
            monsterDetails.SetData(Data);
            monsterDetails.SetMonster(this);
            
            popupManager.RequestPopup(SchemaPopup.PopupType.MonsterDetails);
        }
    }
}
