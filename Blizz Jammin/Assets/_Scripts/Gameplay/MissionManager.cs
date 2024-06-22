using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Gameplay
{
    public class MissionManager : MonoBehaviour
    {
        public enum MissionStatus
        {
            Locked,
            Ready,
            InCombat,
            Complete
        }
        
        // TODO: Merge with Mission.cs??
        public class MissionInfo
        {
            /// <summary>
            /// The world instance of the mission.
            /// </summary>
            public Mission m_mission;

            /// <summary>
            /// The current status of the mission
            /// </summary>
            public MissionStatus m_status;
            
            /// <summary>
            /// The simulation step the mission started.
            /// </summary>
            public int m_startStep;

            /// <summary>
            /// The simulation step the mission will end in. 
            /// </summary>
            public int m_endStep;
            
            /// <summary>
            /// The end score result of the mission.
            /// </summary>
            public float m_score;
            
            // TODO: Cache loot results too?
        }

        [BoxGroup("Lair")]
        [SerializeField] private Transform m_missionRoadRoot;
        [BoxGroup("Lair")]
        [SerializeField] private Mission m_missionPrefab;
        [BoxGroup("Lair")]
        [SerializeField] private Vector3 m_offsetBetweenPages;
        [BoxGroup("Lair")] 
        [SerializeField] private Vector3 m_gap;
        [BoxGroup("Lair")] 
        [SerializeField] private int m_missionsPerPage;

        public Action<MissionInfo> OnMissionStatusChanged;

        private List<MissionInfo> m_missions = new List<MissionInfo>();
        private SchemaGameSettings m_gameSettings;
        
        #region Public

        public MissionInfo GetMissionInfo(SchemaMission mission)
        {
            if (mission == null)
            {
                return null;
            }
            
            if (mission.WorldOrder < 0)
            {
                return null;
            }
            
            if (mission.WorldOrder >= m_missions.Count)
            {
                return null;
            }

            return m_missions[mission.WorldOrder];
        }

        /// <summary>
        /// Gets the previous mission to the given mission. Null if its the first mission.
        /// </summary>
        public MissionInfo GetPrevMissionInfo(SchemaMission mission)
        {
            if (mission.WorldOrder <= 0)
            {
                return null;
            }

            return m_missions[mission.WorldOrder - 1];
        }

        /// <summary>
        /// Gets the next mission to the given mission. Null if its the last mission.
        /// </summary>
        public MissionInfo GetNextMissionInfo(SchemaMission mission)
        {
            if (mission.WorldOrder >= m_missions.Count - 1)
            {
                return null;
            }

            return m_missions[mission.WorldOrder + 1];
        }
        
        /// <summary>
        /// Returns if the given mission can currently start.
        /// It can be prevented by:
        ///   Being busy already, or
        ///   Not enough party members assigned to the mission
        /// </summary>
        public bool CanStartMission(SchemaMission mission)
        {
            var missionInfo = GetMissionInfo(mission);
            if (missionInfo == null)
            {
                return false;
            }
            
            // If the mission is not ready, it cannot begin (locked or busy, etc)
            if (missionInfo.m_status != MissionStatus.Ready)
            {
                return false;
            }
            
            // You must have at least 1 party member to start a mission
            var party = ServiceLocator.Instance.MonsterManager.GetParty(mission);
            for (var i = 0; i < party.Length; i++)
            {
                if (party[i] != null)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public bool IsAnyMissionInCombat()
        {
            foreach (var missionInfo in m_missions)
            {
                if (missionInfo.m_status == MissionStatus.InCombat)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Start the given mission with the current party associated with it.
        /// </summary>
        public void StartMission(SchemaMission mission)
        {
            if (!CanStartMission(mission))
            {
                return;
            }
            
            var missionInfo = m_missions[mission.WorldOrder];
            missionInfo.m_startStep = ServiceLocator.Instance.TimeManager.Day.Value;
            
            (int endStep, float score) = Simulate(missionInfo.m_startStep, mission);

            missionInfo.m_status = MissionStatus.InCombat;
            missionInfo.m_endStep = endStep;
            missionInfo.m_score = score;
            
            OnMissionStatusChanged?.Invoke(missionInfo);
        }
        
        public int GetUnclaimedRewardCount()
        {
            return m_missions.Count(m => m.m_status == MissionStatus.Complete);
        }
        
        public void ClaimRewards(SchemaMission mission)
        {
            var score = GetMissionInfo(mission).m_score;
            
            var party = ServiceLocator.Instance.MonsterManager.GetParty(mission);
            var validMemberCount = party.Count(member => member != null);
            
            var totalLuck = GetAggregatePartyStatValue(SchemaStat.Stat.Luck, party);
            var totalTerror = GetAggregatePartyStatValue(SchemaStat.Stat.Terror, party);
            var totalSymbiosis = GetAggregatePartyStatValue(SchemaStat.Stat.Symbiosis, party);
            
            // Distribute loot after determining roll count through party luck
            // TODO: GRANT LOOT
            var lootTableEntry = mission.LootTable.GetLootTableEntry(score);
            var rolls = lootTableEntry.Rolls + (int)(totalLuck * m_gameSettings.ExtraLootPerLuck);

            // Distribute XP evenly to party members, after adding the party bonus from symbiosis
            int totalXp = (int) (score * mission.Xp * (1 + totalSymbiosis * m_gameSettings.XpScalarPerSymbiosis));
            int xp = totalXp / validMemberCount;
            foreach (var partyMember in party)
            {
                partyMember?.AddXp(xp);
            }
            
            // Add Infamy, after adding the infamy bonus from Terror
            int totalInfamy = (int) (score * mission.Infamy * (1 + totalTerror * m_gameSettings.InfamyScalarPerTerror));
            ServiceLocator.Instance.DeltaInfamy(totalInfamy);

            // Change the status of the mission back to ready, and then inform all listeners.
            m_missions[mission.WorldOrder].m_status = MissionStatus.Ready;
            OnMissionStatusChanged.Invoke(m_missions[mission.WorldOrder]);
            
            // Unlock the next mission if there is one and its locked and the mission was 100% complete
            var nextMission = GetNextMissionInfo(mission);
            if (score >= 1.0f - Double.Epsilon && 
                nextMission != null && 
                nextMission.m_status == MissionStatus.Locked
            ) {
                UnlockMission(nextMission);
            }
            
            // TODO: Localize to MonsterManager, via event?
            var lockedMonsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Locked);
            foreach (var lockedMonster in lockedMonsters)
            {
                if (lockedMonster.Data.UnlockMission != mission)
                {
                    continue;
                }
                
                ServiceLocator.Instance.MonsterManager.Unlock(lockedMonster);
            }
        }

        public void UnlockMission(MissionInfo mission)
        {
            mission.m_status = MissionStatus.Ready;
            OnMissionStatusChanged?.Invoke(mission);
        }

        #endregion

        #region Private

        private void Awake()
        {
            m_gameSettings = ServiceLocator.Instance.GameSettings;
            
            Vector3 offset = Vector3.zero;
            foreach (var mission in ServiceLocator.Instance.AllMissions)
            {
                var missionInstance = Instantiate(m_missionPrefab, m_missionRoadRoot);
                missionInstance.transform.localPosition += offset;
                missionInstance.SetData(mission);

                var missionInfo = new MissionInfo()
                {
                    m_mission = missionInstance,
                    m_status = mission.IsStarter ? MissionStatus.Ready : MissionStatus.Locked
                };

                OnMissionStatusChanged?.Invoke(missionInfo);
                m_missions.Add(missionInfo);
                
                offset += m_gap;
                if (m_missions.Count % m_missionsPerPage == 0)
                {
                    offset =  m_offsetBetweenPages * m_missions.Count / m_missionsPerPage;
                }
            }

            ServiceLocator.Instance.TimeManager.Day.OnChangedValues += OnDayChanged;
        }

        private void OnDayChanged(int _, int day)
        {
            foreach (var missionInfo in m_missions)
            {
                if (missionInfo.m_status != MissionStatus.InCombat)
                {
                    continue;
                }

                if (missionInfo.m_endStep <= day)
                {
                    missionInfo.m_status = MissionStatus.Complete;
                    OnMissionStatusChanged.Invoke(missionInfo);
                }
            }
        }
        
        /// <summary>
        /// Given the mission, returns the simulation result using the party from MonsterManager.
        /// NOTE: Currently, random is not guided so re-running the same calculation might result in a different result.
        ///       We can update as we go forward, but try to not re-run the result unless you understand that quirk.
        ///
        /// int: The end step
        /// float: The score result (0-1)
        /// TODO: Make a struct for this return pair?
        /// </summary>
        private (int, float) Simulate(int startStep, SchemaMission mission)
        {
            // Get the party for the mission
            var party = ServiceLocator.Instance.MonsterManager.GetParty(mission);

            // Calculate the end time of the mission
            // The minimum amount of time a mission can take is 1 day
            int terrorReduction = GetAggregatePartyStatValue(SchemaStat.Stat.Terror, party) /
                                  m_gameSettings.MissionSpeedTerrorPerDay;
            int endStep = startStep + Math.Max(1, mission.Days - terrorReduction);
            
            // Calculate the damage the party will do. Cast to float so that we can get a ratio
            float damage = GetAggregatePartyDamage(party);
            float score = Math.Min(damage / mission.Endurance, 1.0f);

            return (endStep, score);
        }

        // TODO: Move this to a better home?
        private int GetAggregatePartyStatValue(SchemaStat.Stat stat, Monster[] party)
        {
            int value = 0;
            foreach (var monsterInfo in party)
            {
                if (monsterInfo == null)
                {
                    continue;
                }
                
                value += monsterInfo.GetStatValue(stat);
            }
            return value;
        }

        private int GetAggregatePartyDamage(Monster[] party)
        {
            int GetActiveCombatants(int[] enduranceTrackers)
            {
                int activeCombatants = 0;
                foreach (var endurance in enduranceTrackers)
                {
                    activeCombatants += endurance > 0 ? 1 : 0;
                }
                return activeCombatants;
            }
            
            
            // Track how many simulation steps that each party member can do
            int[] partyMemberEnduranceTracker = new int[party.Length];
            for (var i = 0; i < partyMemberEnduranceTracker.Length; i++)
            {
                partyMemberEnduranceTracker[i] = party[i] == null
                    ? 0
                    : party[i].GetStatValue(SchemaStat.Stat.Endurance);
            }

            int damage = 0;
            int activeCombatants = GetActiveCombatants(partyMemberEnduranceTracker);
            while (activeCombatants > 0)
            {
                for (var i = 0; i < partyMemberEnduranceTracker.Length; i++)
                {
                    // This teammate is exhausted
                    if (partyMemberEnduranceTracker[i] <= 0)
                    {
                        continue;
                    }
                    
                    // Remove 1 endurance
                    partyMemberEnduranceTracker[i]--;
                    
                    // Calculate damage this step
                    var monster = party[i];
                    var attack = monster.GetStatValue(SchemaStat.Stat.Attack);
                    var luck = monster.GetStatValue(SchemaStat.Stat.Luck);
                    var terror = monster.GetStatValue(SchemaStat.Stat.Terror);
                    var symbiosis = monster.GetStatValue(SchemaStat.Stat.Symbiosis);

                    var offensiveValue = (int) (attack + activeCombatants * symbiosis * m_gameSettings.DamageBonusPerSymbiosis);
                    var critChance = Math.Min(1.0f, luck / m_gameSettings.CritChanceCap);
                    var isCrit = Random.Range(0f, 1f) <= critChance;
                    var surplusLuck = luck > m_gameSettings.CritChanceCap ? luck - m_gameSettings.CritChanceCap : 0;
                    var critScalar = m_gameSettings.DefaultCritScalar + (terror * m_gameSettings.CritScalarPerTerror) +
                                     (surplusLuck * m_gameSettings.CritScalarPerSurplusLuck);

                    damage += (int) (offensiveValue * (isCrit ? critScalar : 1f));
                }

                // Now that everyone has done their step, their endurances have gone down, we can re-calculate how
                // many monsters are still in combat. It's important to do this last because Symbiosis needs to take 
                // all members into account, and not in any specific order
                activeCombatants = GetActiveCombatants(partyMemberEnduranceTracker);
            }

            return damage;
        }

        #endregion
    }
}
