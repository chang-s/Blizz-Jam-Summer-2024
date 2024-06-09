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
        
        [BoxGroup("Party")]
        [SerializeField] private Sprite m_addMonsterSprite;
        [BoxGroup("Party")] 
        [SerializeField] private UIPartyMonster[] m_partyMembers;
        
        [BoxGroup("Roster")] 
        [SerializeField] private Transform m_monstersRoot;
        [BoxGroup("Roster")] 
        [SerializeField] private UIPartyMonster m_monsterPrefab;

        /// <summary>
        /// This is the index of the last party button that was pressed. Null if no press has occurred yet.
        /// </summary>
        private int? m_lastButtonPressedIndex;

        private Dictionary<Monster, UIPartyMonster> m_monsterPartyInstances = new Dictionary<Monster, UIPartyMonster>();

        public void UpdateRoster()
        {
            var allMonsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters();
            var availableMonsters = ServiceLocator.Instance.MonsterManager.GetMonstersAvailableForMission();
            
            // Make an entry for all monsters. Try to recycle them if they exist already
            foreach (Monster monster in allMonsters)
            {
                if (!m_monsterPartyInstances.ContainsKey(monster))
                {
                    UIPartyMonster partyMonster = Instantiate(m_monsterPrefab, m_monstersRoot);
                    partyMonster.SetData(monster.Data);
                    
                    m_monsterPartyInstances.Add(monster, partyMonster);
                }
                
                // TODO: Set state for in party/selected/available. Need party system
            }
        }
        
        public void SetData(SchemaMission data)
        {
            // Populate the roster
            UpdateRoster();
            
            // Mission details
            m_name.SetText(data.Name);
            m_endurance.SetText(string.Format(c_enduranceFormat, data.Endurance));
            m_time.SetText(string.Format(c_timeFormat, data.Days));
            m_infamy.SetText(string.Format(c_infamyFormat, data.Infamy));
            m_xp.SetText(string.Format(c_xpFormat, data.Xp));
            m_icon.sprite = data.Icon;

            // Monster party
            // Hide any buttons that can't be used
            for (int i = 0; i < m_partyMembers.Length; i++)
            {
                bool canUseMonster = i < data.MaxCapacity;
                m_partyMembers[i].gameObject.SetActive(canUseMonster);
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
