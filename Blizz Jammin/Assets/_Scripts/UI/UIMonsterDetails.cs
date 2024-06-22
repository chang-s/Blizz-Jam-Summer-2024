using System.Linq;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMonsterDetails : 
        SerializedMonoBehaviour, 
        IWorldInstanceController<Monster>, 
        ISchemaController<SchemaMonster>
    {
        private const string c_levelFormat = "Lv {0}";
        private const string c_xpFormat = "{0}/{1}";
        private const string c_xpMax = "MAX";
        
        [BoxGroup("Visuals")]
        [SerializeField] [CanBeNull] private TMP_Text m_name;
        [BoxGroup("Visuals")]
        [SerializeField] [CanBeNull] private TMP_Text m_description;
        [BoxGroup("Visuals")]
        [SerializeField] [CanBeNull] private Image m_icon;
        
        [BoxGroup("XP and Levels")]
        [SerializeField] [CanBeNull] private TMP_Text m_levelLabel;
        [BoxGroup("XP and Levels")]
        [SerializeField] [CanBeNull] private TMP_Text m_xpLabel;

        [BoxGroup("Stats")]
        [SerializeField] [CanBeNull] private UIMonsterStats m_stats;

        [BoxGroup("Quirks")] 
        [SerializeField] [CanBeNull] private Image m_class;
        [BoxGroup("Quirks")] 
        [SerializeField] [CanBeNull]  private Image[] m_quirks;
        
        public void SetInstance(Monster monster)
        {
            // Static data
            SetData(monster.Data);
            
            // Inform the stats subview
            m_stats?.SetInstance(monster);
            
            // Handle XP and Level
            string xpString = c_xpFormat;
            var xpTables = ServiceLocator.Instance.GameSettings.XpForLevel;
            xpString = monster.Level > xpTables.Length 
                ? "MAX" 
                : string.Format(xpString, monster.Xp, xpTables[monster.Level - 1]);
            
            m_xpLabel?.SetText(xpString);
            m_levelLabel?.SetText(string.Format(c_levelFormat, monster.Level.ToString()));
            
            // Handle active Quirks.
            // TODO: Protect this better - We assume the max amount of quirks is 5
            var quirks = monster.Quirks.ToArray();
            for (var i = 0; i < m_quirks?.Length; i++)
            {
                if (i >= quirks.Length)
                {
                    m_quirks[i].gameObject.SetActive(false);
                    continue;
                }
                
                m_quirks[i].gameObject.SetActive(true);
                m_quirks[i].sprite = quirks[i].Icon;
            }
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
            
            // Handle the class icon
            if (m_class != null)
            {
                m_class.sprite = data.Class.Icon;
            }
        }
    }
}
