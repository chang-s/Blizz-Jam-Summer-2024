using System.Collections.Generic;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMonsterDetails : MonoBehaviour, ISchemaController<SchemaMonster>
    {
        [SerializeField] [CanBeNull] private TMP_Text m_name;
        [SerializeField] [CanBeNull] private TMP_Text m_description;
        [SerializeField] [CanBeNull] private Image m_icon;
        [SerializeField] private UIStat m_prefab;

        [BoxGroup("XP and Levels")]
        [SerializeField] [CanBeNull] private TMP_Text m_level;
        [BoxGroup("XP and Levels")]
        [SerializeField] private string m_levelFormat;
        
        private List<UIStat> m_statInstances = new List<UIStat>();

        // TODO: Find a better way to formalize setting dynamic data. Maybe there is a
        // world instance base type, and we have this be IWorldInstanceController or something,
        // instead of doing Schema, since its not enough for monsters
        public void SetMonster(Monster monster)
        {
            // Static data
            SetData(monster.Data);
            
            // Dynamic data
            string levelString = m_levelFormat;
            var xpTables = ServiceLocator.Instance.GameSettings.XpForLevel;
            levelString = monster.Level >= xpTables.Length 
                ? "MAX" 
                : string.Format(m_levelFormat, monster.Xp, xpTables[monster.Level - 1]);
            m_level?.SetText(levelString);
        }
        
        public void SetData(SchemaMonster data)
        {
            if (data == null)
            {
                return;
            }
            
            // Handle the image
            if (m_icon != null)
            {
                m_icon.sprite = data.Sprite;
            }

            // Handle texts
            m_name?.SetText(data.Name);
            m_description?.SetText(data.Description);
        }
    }
}
