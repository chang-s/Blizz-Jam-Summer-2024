using System.Collections.Generic;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMissionDetails : MonoBehaviour, ISchemaController<SchemaMission>
    {
        private const string c_enduranceFormat = "Endurance: {0}";
        private const string c_timeFormat = "Time: {0}";
        private const string c_infamyFormat = "Infamy: {0}";
        private const string c_xpFormat = "XP: {0}";
        
        [BoxGroup("Mission State")] 
        [SerializeField] private GameObject m_missionState;
        [BoxGroup("Mission State")] 
        [SerializeField] private TMP_Text m_name;
        [BoxGroup("Mission State")] 
        [SerializeField] private TMP_Text m_endurance;
        [BoxGroup("Mission State")] 
        [SerializeField] private TMP_Text m_time;
        [BoxGroup("Mission State")] 
        [SerializeField] private TMP_Text m_infamy;
        [BoxGroup("Mission State")] 
        [SerializeField] private TMP_Text m_xp;
        [BoxGroup("Mission State")] 
        [SerializeField] private Image m_icon;
        [BoxGroup("Mission State")] 
        [SerializeField] private UILoot m_lootPrefab;
        [BoxGroup("Mission State")] 
        [SerializeField] private Transform m_lootRoot;
        
        [BoxGroup("Monsters")]
        [SerializeField] private Sprite m_addMonsterSprite;
        [BoxGroup("Monsters")]
        [SerializeField] private Button[] m_monsterButtons;
        
        [BoxGroup("Add Monster State")]
        [SerializeField] private GameObject m_addMonsterState;
        [BoxGroup("Add Monster State")]
        [SerializeField] private UIMonsterDetails m_monsterDetails;
        [BoxGroup("Add Monster State")] 
        [SerializeField] private Button m_addButton;

        // TODO: Make this some sort of reusable UI component, for item inventory or other uses
        //       Something like "UIPages"
        [BoxGroup("Add Monster State")] 
        [SerializeField] private Button m_leftButton;
        [BoxGroup("Add Monster State")] 
        [SerializeField] private Button m_rightButton;

        /// <summary>
        /// This is the index of the last party button that was pressed. Null if no press has occurred yet.
        /// </summary>
        private int? m_lastButtonPressedIndex;

        /// <summary>
        /// This is the index of the last shown owned monster.
        /// </summary>
        private int m_shownMonsterIndex = 0;
        
        private void Awake()
        {
            for (var i = 0; i < m_monsterButtons.Length; i++)
            {
                var capturedIndex = i;
                m_monsterButtons[i].onClick.AddListener(() =>
                {
                    HandleButtonClicked(capturedIndex);
                });
            }

            m_addButton.onClick.AddListener(OnAddButtonClicked);
            m_leftButton.onClick.AddListener(DecrementMonsterChoice);
            m_rightButton.onClick.AddListener(IncrementMonsterChoice);
        }

        private void HandleButtonClicked(int buttonIndex)
        {
            // TODO: Save state better
            bool wasAddingMonster = m_addMonsterState.activeInHierarchy;
            bool isReClick = buttonIndex == m_lastButtonPressedIndex;
            bool isAddingMonster = !wasAddingMonster || !isReClick;
            m_lastButtonPressedIndex = buttonIndex;
            
            m_missionState.SetActive(!isAddingMonster);
            m_addMonsterState.SetActive(isAddingMonster);
            
            if (!isAddingMonster)
            {
                return;
            }
            
            // Assume at least one monster
            var monsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters();
            if (monsters == null || monsters.Count < m_shownMonsterIndex)
            {
                return;
            }
            
            // TODO: Check for out of bounds
            m_monsterDetails.SetData(monsters[m_shownMonsterIndex].Data);
        }

        private void OnAddButtonClicked()
        {
            // Something went terribly wrong
            if (!m_lastButtonPressedIndex.HasValue)
            {
                return;
            }
            
            // If for some reason we were able to click the button without being in the 
            // adding monster state, we can ignore the click
            bool wasAddingMonster = m_addMonsterState.activeInHierarchy;
            if (!wasAddingMonster)
            {
                return;
            }
            
            var monsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters();
            if (monsters == null || monsters.Count <= m_lastButtonPressedIndex)
            {
                return;
            }
            
            // Add this monster to the party
            // TODO: Save this/creat party system, for now it just changes the icon
            m_monsterButtons[m_lastButtonPressedIndex.Value].image.sprite = monsters[m_shownMonsterIndex].Data.Sprite;
            
            // Go back to the mission screen
            m_missionState.SetActive(true);
            m_addMonsterState.SetActive(false);
        }
        
        private void IncrementMonsterChoice()
        {
            // Assume at least one monster
            var monsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters();
            if (monsters == null || monsters.Count <= m_lastButtonPressedIndex)
            {
                return;
            }

            m_shownMonsterIndex++;
            if (m_shownMonsterIndex == monsters.Count)
            {
                m_shownMonsterIndex = 0;
            }
            m_monsterDetails.SetData(monsters[m_shownMonsterIndex].Data);
        }

        private void DecrementMonsterChoice()
        {
            // Assume at least one monster
            var monsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters();
            if (monsters == null || monsters.Count <= m_lastButtonPressedIndex)
            {
                return;
            }
            
            m_shownMonsterIndex--;
            if (m_shownMonsterIndex < 0)
            {
                m_shownMonsterIndex = monsters.Count - 1;
            }
            m_monsterDetails.SetData(monsters[m_shownMonsterIndex].Data);
        }
        
        public void SetData(SchemaMission data)
        {
            // Mission details
            m_name.SetText(data.Name);
            m_endurance.SetText(string.Format(c_enduranceFormat, data.Endurance));
            m_time.SetText(string.Format(c_timeFormat, data.Days));
            m_infamy.SetText(string.Format(c_infamyFormat, data.Infamy));
            m_xp.SetText(string.Format(c_xpFormat, data.Xp));
            m_icon.sprite = data.Icon;

            // Monster party
            // Hide any buttons that can't be used
            for (int i = 0; i < m_monsterButtons.Length; i++)
            {
                bool canUseMonster = i < data.MaxCapacity;
                m_monsterButtons[i].gameObject.SetActive(canUseMonster);
            }
            
            // TODO: Equip/Unequip Monsters to the party

            // Loot for mission. Clear whatever is there, then instantiate new ones.
            // TODO: Optimization - We can recycle these via a pool
            foreach(Transform child in m_lootRoot.transform)
            {
                Destroy(child.gameObject);
            }

            HashSet<SchemaLoot> loot = data.LootTable.GetAllPossibleLoot();
            foreach (var schemaLoot in loot)
            {
                UILoot instance = Instantiate(m_lootPrefab, m_lootRoot);
                instance.SetData(schemaLoot);
            }
        }
    }
}
