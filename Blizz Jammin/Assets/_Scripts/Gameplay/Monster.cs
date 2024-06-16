using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Schemas;
using _Scripts.UI;
using TMPro;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class Monster : MonoBehaviour, ISchemaController<SchemaMonster>
    {
        public SchemaMonster Data { get; private set; }
        public uint Level { get; private set; } = 1;

        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private TextMeshPro m_nameLabel;

        private Dictionary<SchemaStat.Stat, int> m_stats = new();
        private Dictionary<SchemaStat.Stat, int> m_flatStatBonus = new();
        private Dictionary<SchemaStat.Stat, float> m_mulltStatBonus = new();

        private List<SchemaQuirk> m_unlockedQuirks = new List<SchemaQuirk>();
        private List<SchemaQuirk> m_unlockedClasses = new List<SchemaQuirk>();

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
            //Probably should rework this when level unlocks classes/quirks?
            m_unlockedQuirks.Clear();
            for (int i = 0; i < Level; ++i)
            {
                if(i < Data.PossibleQuirks.Count() && Data.PossibleQuirks[i] != null)
                    m_unlockedQuirks.Add(Data.PossibleQuirks[i]);
            }
            return m_unlockedQuirks;
        }

        public List<SchemaQuirk> GetUnlockedClasses()
        {
            //Probably should rework this when level unlocks classes/quirks?
            m_unlockedClasses.Clear();
            for (int i = 0; i < Level; ++i)
            {
                if (i < Data.PossibleClasses.Count() && Data.PossibleClasses[i] != null)
                    m_unlockedClasses.Add(Data.PossibleClasses[i]);
            }
            return m_unlockedClasses;
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
            
            UIPopup popup = popupManager.GetPopup(UIPopupManager.PopupType.MonsterDetails);
            UIMonsterDetails monsterDetails = popup.GetComponent<UIMonsterDetails>();
            monsterDetails.SetData(Data);
            
            popupManager.RequestPopup(UIPopupManager.PopupType.MonsterDetails);
        }
    }
}
