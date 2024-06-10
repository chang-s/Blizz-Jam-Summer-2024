using System.Collections.Generic;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    // TODO: Abstract out "Party" into a class and have it self-manage? this class is getting huge
    public class UIMissionDetails : MonoBehaviour, ISchemaController<SchemaMission>
    {
        private const string c_enduranceFormat = "Endurance: {0}";
        private const string c_timeFormat = "Time: {0}";
        private const string c_infamyFormat = "Infamy: {0}";
        private const string c_xpFormat = "XP: {0}";

        [BoxGroup("Popup")] 
        [SerializeField] private UIPopup m_popup;
        
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
        [SerializeField] private UIPartyMonster[] m_partyMembers;
        
        [BoxGroup("Roster")] 
        [SerializeField] private Transform m_rosterRoot;
        [BoxGroup("Roster")] 
        [SerializeField] private UIRosterMonster m_rosterMonsterPrefab;

        private enum Mode
        {
            Normal,
            AddingMonster,
        }

        private Mode m_mode = Mode.Normal;
        
        /// <summary>
        /// This is the index of the last party button that was pressed. Null if no press has occurred yet.
        /// </summary>
        private int? m_lastPartyButtonPressedIndex;

        // TODO: Should the key be schema?
        private Dictionary<Monster, UIRosterMonster> m_rosterInstances = new Dictionary<Monster, UIRosterMonster>();
        private SchemaMission m_missionData;

        public void SetData(SchemaMission data)
        {
            m_missionData = data;
            
            // Mission details
            m_name.SetText(data.Name);
            m_endurance.SetText(string.Format(c_enduranceFormat, data.Endurance));
            m_time.SetText(string.Format(c_timeFormat, data.Days));
            m_infamy.SetText(string.Format(c_infamyFormat, data.Infamy));
            m_xp.SetText(string.Format(c_xpFormat, data.Xp));
            m_icon.sprite = data.Icon;

            // Loot
            // Clear whatever is there, then instantiate new ones.
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
        
        private void Awake()
        {
            for (var i = 0; i < m_partyMembers.Length; i++)
            {
                var partyButtonIndex = i;
                m_partyMembers[i].Button.onClick.AddListener(() =>
                {
                    OnPartyButtonClicked(partyButtonIndex);
                });
            }

            ServiceLocator.Instance.MonsterManager.OnPartyChanged += OnPartyChanged;
            m_popup.OnShow += OnShow;

        }

        private void OnShow()
        {
            m_lastPartyButtonPressedIndex = null;
            SetMode(Mode.Normal);
        }

        private void OnPartyButtonClicked(int partyIndex)
        {
            // On reselection, clear the monster
            bool isReSelect = m_lastPartyButtonPressedIndex.HasValue && m_lastPartyButtonPressedIndex == partyIndex;
            if (isReSelect)
            {
                ServiceLocator.Instance.MonsterManager.RemoveMonsterFromParty(
                    m_partyMembers[partyIndex].MonsterData,
                    m_missionData,
                    partyIndex
                );
                
                m_lastPartyButtonPressedIndex = null;
                SetMode(Mode.Normal);
                
                return;
            }
            
            m_lastPartyButtonPressedIndex = partyIndex;
            SetMode(Mode.AddingMonster);
        }

        private void SetMode(Mode mode)
        {
            m_mode = mode;
            UpdateRoster();
            UpdateParty();
        }

        private void OnPartyChanged(SchemaMission mission)
        {
            if (mission != m_missionData)
            {
                return;
            }
            
            UpdateRoster();
            UpdateParty();
        }
        
        private void UpdateRoster()
        {
            var allMonsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters();
            var partyMonsters = ServiceLocator.Instance.MonsterManager.GetParty(m_missionData);

            // Make an entry for all monsters. Try to recycle them if they exist already
            foreach (MonsterManager.MonsterInfo monsterInfo in allMonsters)
            {
                if (!m_rosterInstances.ContainsKey(monsterInfo.m_worldInstance))
                {
                    UIRosterMonster rosterMonster = Instantiate(m_rosterMonsterPrefab, m_rosterRoot);
                    rosterMonster.SetData(monsterInfo.m_worldInstance.Data);

                    rosterMonster.Button.onClick.AddListener(() =>
                    {
                        if (m_mode != Mode.AddingMonster ||  !m_lastPartyButtonPressedIndex.HasValue)
                        {
                            return;
                        }
                        
                        ServiceLocator.Instance.MonsterManager.AddMonsterToParty(
                            rosterMonster.MonsterData,
                            m_missionData,
                            m_lastPartyButtonPressedIndex.Value
                        );

                        m_lastPartyButtonPressedIndex = null;
                        SetMode(Mode.Normal);
                    });

                    m_rosterInstances.Add(monsterInfo.m_worldInstance, rosterMonster);
                }

                // TODO: Clean this logic up 
                bool isAddingMonster = m_mode == Mode.AddingMonster;
                bool isBusy = monsterInfo.m_status == MonsterManager.MonsterStatus.Busy;
                bool isInMissionParty = monsterInfo.m_currentMission == m_missionData;
                bool isCurrentlySelected = m_lastPartyButtonPressedIndex.HasValue &&
                                           partyMonsters[m_lastPartyButtonPressedIndex.Value] != null &&
                                           m_rosterInstances[monsterInfo.m_worldInstance].MonsterData ==
                                           partyMonsters[m_lastPartyButtonPressedIndex.Value].m_worldInstance.Data;
                
                UIRosterMonster.State state = UIRosterMonster.State.Normal;
                if (isBusy && !isInMissionParty)
                {
                    state = UIRosterMonster.State.InCombat;
                }
                else if (isAddingMonster)
                {
                    state = UIRosterMonster.State.Addable;
                    
                    if (isCurrentlySelected)
                    {
                        state = UIRosterMonster.State.Selected;
                    }
                    else if (isInMissionParty)
                    {
                        state = UIRosterMonster.State.InParty;
                    }
                }
                else
                {
                    state = isInMissionParty ? UIRosterMonster.State.InParty : UIRosterMonster.State.Normal;
                }

                m_rosterInstances[monsterInfo.m_worldInstance].SetState(state);
            }
        }

        private void UpdateParty()
        {
            var partyMonsters = ServiceLocator.Instance.MonsterManager.GetParty(m_missionData);
            for (int i = 0; i < m_partyMembers.Length; i++)
            {
                bool canUseMonster = i < m_missionData.MaxCapacity;
                m_partyMembers[i].gameObject.SetActive(canUseMonster);

                if (!canUseMonster)
                {
                    continue;
                }
                
                m_partyMembers[i].SetData(partyMonsters[i]?.m_worldInstance.Data);

                switch (m_mode)
                {
                    case Mode.Normal:
                        m_partyMembers[i].SetState(UIPartyMonster.State.Normal);
                        break;
                    
                    case Mode.AddingMonster:
                        m_partyMembers[i].SetState(m_lastPartyButtonPressedIndex.Value == i 
                            ? UIPartyMonster.State.Selected 
                            : UIPartyMonster.State.Normal
                        );
                        break;
                }
            }
        }
    }
}
