using System.Linq;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using _Scripts.UI.Tooltip;
using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    // TODO: Split this into UIMonster and UIMonsterDetails, its currently serving as both
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
        [BoxGroup("XP and Levels")]
        [SerializeField] [CanBeNull] private Slider m_xpSlider;
        
        [BoxGroup("Stats")]
        [SerializeField] [CanBeNull] private UIMonsterStats m_stats;

        [BoxGroup("Quirks")] 
        [SerializeField] [CanBeNull] private Image m_class;
        [BoxGroup("Quirks")] 
        [SerializeField] [CanBeNull]  private Image[] m_quirks;
        
        // TODO: make a new class to drive this?
        [BoxGroup("Items")] 
        [SerializeField] [CanBeNull]  private Button[] m_itemButtons;
        [SerializeField] [CanBeNull]  private Image[] m_itemImages;

        private Monster m_instance;

        private void Awake()
        {
            if (m_itemButtons != null)
            {
                for (var i = 0; i < m_itemButtons.Length; i++)
                {
                    var capturedIndex = i;
                    m_itemButtons[i].onClick.AddListener(() =>
                    {
                        OnItemButtonClicked(capturedIndex);
                    });
                }
            }
            
            ServiceLocator.Instance.LootManager.OnLootEquipped += _ => UpdateLoot();
            ServiceLocator.Instance.LootManager.OnLootUnEquipped += _ => UpdateLoot();
        }

        public void SetInstance(Monster instance)
        {
            m_instance = instance;
            
            // Static data
            SetData(instance.Data);
            
            // Inform the stats subview
            m_stats?.SetInstance(instance);
            
            // Handle XP and Level
            string xpString = c_xpFormat;
            var xpTables = ServiceLocator.Instance.GameSettings.XpForLevel;
            xpString = instance.Level > xpTables.Length 
                ? c_xpMax
                : string.Format(xpString, instance.Xp, xpTables[instance.Level - 1]);

            if (m_xpSlider != null)
            {
                float xpRatio = 1.0f;
                m_xpSlider.minValue = 0;
                m_xpSlider.maxValue = 1;
                if (instance.Level <= xpTables.Length)
                {
                    m_xpSlider.maxValue = xpTables[instance.Level - 1];
                    xpRatio = (float)instance.Xp / xpTables[instance.Level - 1];
                }
                m_xpSlider.normalizedValue = xpRatio;
            }
            
            
            m_xpLabel?.SetText(xpString);
            m_levelLabel?.SetText(string.Format(c_levelFormat, instance.Level.ToString()));
            
            // Handle active Quirks.
            // TODO: Protect this better - We assume the max amount of quirks is 5
            var quirks = instance.Quirks.ToArray();
            for (var i = 0; i < m_quirks?.Length; i++)
            {
                if (i >= quirks.Length)
                {
                    m_quirks[i].gameObject.SetActive(false);
                    continue;
                }
                
                m_quirks[i].gameObject.SetActive(true);
                m_quirks[i].sprite = quirks[i].Icon;
                m_quirks[i].GetComponent<UITooltipRequesterSchema>().SetSchema(quirks[i]);
            }

            UpdateLoot();
        }

        private void UpdateLoot()
        {
            if (m_itemImages == null || m_itemImages.Length == 0)
            {
                return;
            }
            
            for (var i = 0; i < m_itemImages.Length; i++)
            {
                var equippedLoot = m_instance.EquippedLoot.Count > i
                    ? m_instance.EquippedLoot[i]
                    : null;
                
                // TODO: Better UX indexing items
                // TODO: better "add item" sprite
                m_itemImages[i].sprite = equippedLoot?.Data.Icon;
                m_itemImages[i].gameObject.SetActive(equippedLoot != null);
            }

            // LootInstances changed, so we must update the stat panel
            if (m_instance != null && m_stats != null)
            {
                m_stats.SetInstance(m_instance);
            }
        }

        // TODO: use index to drive better UX
        private void OnItemButtonClicked(int itemIndex)
        {
            var popupManager = ServiceLocator.Instance.UIPopupManager;
            var equipPopup = popupManager
                .GetPopup(SchemaPopup.PopupType.Equip)
                .GetComponent<UILootEquipPopup>();
            
            equipPopup.SetInstance(m_instance);
            
            popupManager.RequestPopup(SchemaPopup.PopupType.Equip);
            
            ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.ButtonClick);
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
                m_class.GetComponent<UITooltipRequesterSchema>()?.SetSchema(data.Class);
            }
        }
    }
}
