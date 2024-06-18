using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    // TODO: Abstract out "Party" into a class and have it self-manage? this class is getting huge
    // TODO: Major cleanup on monste add flow
    public class UIMissionDetails : MonoBehaviour, ISchemaController<SchemaMission>
    {
        private const string c_enduranceFormat = "Endurance: {0}";
        private const string c_timeFormat = "Time: {0}";
        private const string c_infamyFormat = "Infamy: {0}";
        private const string c_xpFormat = "XP: {0}";
        private const string c_difficultyDefault = "Medium";
        private const string c_quoteDefault = "Wow, is it me or is it kinda hot in here?";

        [BoxGroup("Popup")] 
        [SerializeField] private UIPopup m_popup;
        
        [BoxGroup("Mission State")] 
        [SerializeField] private TMP_Text m_name;
        [BoxGroup("Mission State")]
        [SerializeField] private TMP_Text m_difficulty;
        [BoxGroup("Mission State")]
        [SerializeField] private TMP_Text m_quote;
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
        [BoxGroup("Mission State")]
        [SerializeField] private Transform m_classEffectRoot;
        [BoxGroup("Mission State")]
        [SerializeField] private Transform m_quirkEffectRoot;
        [BoxGroup("Mission State")]
        [SerializeField] private UIMissionEffect m_effectPrefab;

        [BoxGroup("Party")] 
        [SerializeField] private UIPartyMonster[] m_partyMembers;
        
        [BoxGroup("Roster")] 
        [SerializeField] private Transform m_rosterRoot;
        [SerializeField] private TMP_Dropdown m_sortDropdown;

        [BoxGroup("Roster")] 
        [SerializeField] private UIRosterMonster m_rosterMonsterPrefab;

        [BoxGroup("Simulation")] 
        [SerializeField] private Button m_start;

        private enum Mode
        {
            Normal,
            AddingMonster,
        }

        private Mode m_mode = Mode.Normal;
        
        /// <summary>
        /// The index of the party that we are currently manipulating. Null if we are not in Add mode.
        /// </summary>
        private int? m_currentPartyIndex;

        // TODO: Should the key be schema?
        private Dictionary<Monster, UIRosterMonster> m_rosterInstances = new Dictionary<Monster, UIRosterMonster>();
        private SchemaMission m_missionData;

        public void SetData(SchemaMission data)
        {
            m_missionData = data;
            
            // Mission details
            m_name.SetText(data.Name);
            
            // This is just UX temp. There is no string denoting the difficulty of the mission
            // What should be shown is the Endurance (or Health, whatever we end up calling it)
            // If we wanna call it difficulty, sure, but its going to be an Integer
            /*
            if (data.Difficulty != string.Empty)
                m_difficulty.SetText(data.Difficulty);
            else
                m_difficulty.SetText(c_difficultyDefault);
            */

            m_quote.SetText(data.Quote != string.Empty ? data.Quote : c_quoteDefault);

            // TODO: Hook up these two
            //m_endurance.SetText(string.Format(c_enduranceFormat, data.Endurance));
            //m_time.SetText(string.Format(c_timeFormat, data.Days));
            
            m_infamy.SetText(data.Infamy.ToString());
            m_xp.SetText(data.Xp.ToString());
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

            foreach (Transform child in m_classEffectRoot.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in m_quirkEffectRoot.transform)
            {
                Destroy(child.gameObject);
            }
            

            var mods = ServiceLocator.Instance.MissionManager
                .GetMissionInfo(data)
                .m_worldInstance.Modifiers;
            
            if (mods != null)
            {
                foreach (var quirk in mods)
                {
                    if (quirk.Quirk != null)
                    {
                        UIMissionEffect quirkEffect = Instantiate(m_effectPrefab, m_quirkEffectRoot);
                        quirkEffect.SetData(quirk);
                    }
                }
            }

            m_start.interactable = ServiceLocator.Instance.MissionManager.CanStartMission(m_missionData);
        }
        
        private void Awake()
        {
            m_start.onClick.AddListener(() =>
            {
                var missionManager = ServiceLocator.Instance.MissionManager;
                if (!missionManager.CanStartMission(m_missionData))
                {
                    return;
                }

                missionManager.StartMission(m_missionData);
            });
            
            for (var i = 0; i < m_partyMembers.Length; i++)
            {
                var partyButtonIndex = i;
                m_partyMembers[i].Button.onClick.AddListener(() =>
                {
                    OnPartyButtonClicked(partyButtonIndex);
                });
            }

            m_sortDropdown.onValueChanged.AddListener(delegate { UpdateRoster(); });

            ServiceLocator.Instance.MonsterManager.OnPartyChanged += OnPartyChanged;
            ServiceLocator.Instance.MissionManager.OnMissionStatusChanged += OnMissionStatusChanged;
            m_popup.OnShow += OnShow;
        }

        private List<MonsterManager.MonsterInfo> SortMonsters(List<MonsterManager.MonsterInfo> monsters)
        {
            var sortedMonsters = monsters;

            switch (m_sortDropdown.value)
            {
                case 0: //Sorts by Name
                    sortedMonsters = sortedMonsters.OrderBy(m => m.m_worldInstance.Data.Name).ToList();
                    break;
                case 1: //Sorts by Class
                    sortedMonsters = sortedMonsters.OrderBy(m => m.m_worldInstance.GetClass().Name).ToList();
                    break;
                case 2: //Sorts by Level
                    sortedMonsters = sortedMonsters.OrderByDescending(m => m.m_worldInstance.Level).ToList();
                    break;
                case 3: //Sorts by Attack
                    sortedMonsters = sortedMonsters.OrderByDescending(m => m.m_worldInstance.GetStatValue(SchemaStat.Stat.Attack)).ToList();
                    break;
                case 4: //Sort by Endurance
                    sortedMonsters = sortedMonsters.OrderByDescending(m => m.m_worldInstance.GetStatValue(SchemaStat.Stat.Endurance)).ToList();
                    break;
                case 5: //Sorts by Luck 
                    sortedMonsters = sortedMonsters.OrderByDescending(m => m.m_worldInstance.GetStatValue(SchemaStat.Stat.Luck)).ToList();
                    break;
                case 6: //Sorts by Symbiosis 
                    sortedMonsters = sortedMonsters.OrderByDescending(m => m.m_worldInstance.GetStatValue(SchemaStat.Stat.Symbiosis)).ToList();
                    break;
                case 7: //Sorts by Terror 
                    sortedMonsters = sortedMonsters.OrderByDescending(m => m.m_worldInstance.GetStatValue(SchemaStat.Stat.Terror)).ToList();
                    break;
            }
            return sortedMonsters;
        }

        private void OnMissionStatusChanged(MissionManager.MissionInfo missionInfo)
        {
            if (missionInfo.m_mission != m_missionData)
            {
                return;
            }
            
            // Special case: If the mission we are inspecting finishes while we're inspecting it,
            // automatically hide this popup and then show the results
            if (m_popup.Showing && missionInfo.m_status == MissionManager.MissionStatus.Complete)
            {
                var resultsPopup = ServiceLocator.Instance.UIPopupManager
                    .GetPopup(SchemaPopup.PopupType.MissionResults).GetComponent<UIMissionResults>();
                resultsPopup.SetData(missionInfo.m_mission);
                ServiceLocator.Instance.UIPopupManager.RequestClose();
                ServiceLocator.Instance.UIPopupManager.RequestPopup(SchemaPopup.PopupType.MissionResults);
                return;
            }
            
            UpdateRoster();
            UpdateParty();
        }

        private void OnShow()
        {
            SetMode(Mode.Normal);
        }

        private void OnPartyButtonClicked(int partyIndex)
        {
            // On reselection, clear the monster
            bool isReSelect = m_currentPartyIndex.HasValue && m_currentPartyIndex == partyIndex;
            if (isReSelect)
            {
                SetMode(Mode.Normal);
                return;
            }
            
            m_currentPartyIndex = partyIndex;
            SetMode(Mode.AddingMonster);
        }

        private void SetMode(Mode mode)
        {
            m_mode = mode;

            switch (m_mode)
            {
                case Mode.Normal:
                    m_currentPartyIndex = null;
                    break;
            }
            
            UpdateRoster();
            UpdateParty();
        }

        // TODO: Might not need this?
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
            var sortedMonsters = SortMonsters(allMonsters);
            var partyMonsters = ServiceLocator.Instance.MonsterManager.GetParty(m_missionData);

            //Super unoptimal - don't judge me ;)
            foreach (var instance in m_rosterInstances.Values)
            {
                Destroy(instance.gameObject);
            }
            m_rosterInstances.Clear();

            // Make an entry for all monsters. Try to recycle them if they exist already
            foreach (MonsterManager.MonsterInfo monsterInfo in sortedMonsters)
            {
                if (!m_rosterInstances.ContainsKey(monsterInfo.m_worldInstance))
                {
                    UIRosterMonster rosterMonster = Instantiate(m_rosterMonsterPrefab, m_rosterRoot);
                    rosterMonster.SetData(monsterInfo.m_worldInstance.Data);
                    rosterMonster.Button.onClick.AddListener(() => OnRosterEntryClicked(rosterMonster));
                    m_rosterInstances.Add(monsterInfo.m_worldInstance, rosterMonster);
                }

                // TODO: Clean this logic up 
                bool isAddingMonster = m_mode == Mode.AddingMonster;
                bool isBusy = monsterInfo.m_status == MonsterManager.MonsterStatus.Busy;
                bool isInMissionParty = monsterInfo.m_currentMission == m_missionData;
                bool isCurrentlySelected = m_currentPartyIndex.HasValue &&
                                           partyMonsters[m_currentPartyIndex.Value] != null &&
                                           m_rosterInstances[monsterInfo.m_worldInstance].MonsterData ==
                                           partyMonsters[m_currentPartyIndex.Value].m_worldInstance.Data;
                
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

        private void OnRosterEntryClicked(UIRosterMonster monster)
        {
            if (m_mode != Mode.AddingMonster ||  !m_currentPartyIndex.HasValue)
            {
                return;
            }
            
            var party = ServiceLocator.Instance.MonsterManager.GetParty(m_missionData);
            bool isCurrentlySelected = party[m_currentPartyIndex.Value] != null &&
                                       monster.MonsterData == party[m_currentPartyIndex.Value].m_worldInstance.Data;

            int oldPartyIndex = m_currentPartyIndex.Value;
            
            // Remove current entry, do not advance
            if (isCurrentlySelected)
            {
                ServiceLocator.Instance.MonsterManager.RemoveMonsterFromParty(
                    monster.MonsterData,
                    m_missionData,
                    oldPartyIndex
                );
            }
            // Add current entry, advance if possible
            else
            {
                ServiceLocator.Instance.MonsterManager.AddMonsterToParty(
                    monster.MonsterData,
                    m_missionData,
                    oldPartyIndex
                );
                
                // There are no more monsters available, exit out early
                if (AreNoMonstersAvailable())
                {
                    SetMode(Mode.Normal);
                    return;
                }
                
                // Party is full, exit out early
                if (IsPartyFull())
                {
                    SetMode(Mode.Normal);
                    return;
                }
                
                // Advance to the next empty index
                while (party[m_currentPartyIndex.Value] != null)
                {
                    m_currentPartyIndex++;
                    if (m_currentPartyIndex >= m_missionData.MaxCapacity)
                    {
                        m_currentPartyIndex = 0;
                    }
                }

                UpdateRoster();
                UpdateParty();
            }
        }

        private void UpdateParty()
        {
            var partyMonsters = ServiceLocator.Instance.MonsterManager.GetParty(m_missionData);
            for (int i = 0; i < m_partyMembers.Length; i++)
            {
                bool canUseMonster = i < m_missionData.MaxCapacity;
                if (!canUseMonster)
                {
                    m_partyMembers[i].SetState(UIPartyMonster.State.Locked);
                    continue;
                }
                
                m_partyMembers[i].SetData(partyMonsters[i]?.m_worldInstance.Data);

                switch (m_mode)
                {
                    case Mode.Normal:
                        m_partyMembers[i].SetState(UIPartyMonster.State.Normal);
                        break;
                    
                    case Mode.AddingMonster:
                        m_partyMembers[i].SetState(m_currentPartyIndex.Value == i 
                            ? UIPartyMonster.State.Selected 
                            : UIPartyMonster.State.Normal
                        );
                        break;
                }
            }
            
            m_start.interactable = ServiceLocator.Instance.MissionManager.CanStartMission(m_missionData);
        }
        
        // TODO: Move to MonsterManager?
        private bool IsPartyFull()
        {
            var party = ServiceLocator.Instance.MonsterManager.GetParty(m_missionData);
            for (var i = 0; i < party.Length; i++)
            {
                if (party[i] == null)
                {
                    return false;
                }
            }

            return true;
        }
        
        // TODO: Move to MonsterManager?
        private bool AreNoMonstersAvailable()
        {
            var allMonsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters();
            foreach (var monsterInfo in allMonsters)
            {
                if (monsterInfo.m_status == MonsterManager.MonsterStatus.Purchased)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
