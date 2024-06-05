using System;
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
        }

        private void HandleButtonClicked(int buttonIndex)
        {
            // TODO:
            m_missionState.SetActive(!m_missionState.activeInHierarchy);
            m_addMonsterState.SetActive(!m_addMonsterState.activeInHierarchy);

            var monster = ServiceLocator.Instance.MonsterManager.GetMonster(buttonIndex);
            m_monsterDetails.SetData(monster.Data);
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
