using System;
using System.Collections.Generic;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;

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
        
        public class MissionInfo
        {
            /// <summary>
            /// The world instance of the mission.
            /// </summary>
            public Mission m_worldInstance;
            
            /// <summary>
            /// The base data for the given mission.
            /// </summary>
            public SchemaMission m_mission;

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

        [BoxGroup("World Missions")]
        [SerializeField] private Transform m_missionRoadRoot;
        [SerializeField] private Mission m_missionPrefab;
        [SerializeField] private Vector3 m_offsetBetweenPages;
        [SerializeField] private Transform[] m_pageOffsets;

        public Action<MissionInfo> OnMissionStatusChanged;
        
        private Dictionary<SchemaMission, MissionInfo> m_missions = new Dictionary<SchemaMission, MissionInfo>();
        private SchemaGameSettings m_gameSettings;
        
        #region Public
        
        /// <summary>
        /// Returns if the given mission can currently start.
        /// It can be prevented by:
        ///   Being busy already, or
        ///   Not enough party members assigned to the mission
        /// </summary>
        public bool CanStartMission(SchemaMission mission)
        {
            if (mission == null)
            {
                return false;
            }

            if (!m_missions.ContainsKey(mission))
            {
                return false;
            }

            // If the mission is not ready, it cannot begin (locked or busy, etc)
            var missionInfo = m_missions[mission];
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
            foreach (var (mission, missionInfo) in m_missions)
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
            
            var missionInfo = m_missions[mission];
            missionInfo.m_startStep = ServiceLocator.Instance.TimeManager.Day.Value;
            
            (int endStep, float score) = Simulate(missionInfo.m_startStep, mission);

            missionInfo.m_status = MissionStatus.InCombat;
            missionInfo.m_endStep = endStep;
            missionInfo.m_score = score;
            
            OnMissionStatusChanged?.Invoke(missionInfo);
        }

        #endregion

        #region Private

        private void Awake()
        {
            m_gameSettings = ServiceLocator.Instance.GameSettings;
            
            int missionWorldIndex = 0;
            foreach (var mission in ServiceLocator.Instance.AllMissions)
            {
                var missionInstance = Instantiate(m_missionPrefab, m_missionRoadRoot);
                missionInstance.SetData(mission);
                
                int page = missionWorldIndex / m_pageOffsets.Length;
                int offsetIndex = missionWorldIndex % m_pageOffsets.Length;
                missionInstance.transform.localPosition += (page * m_offsetBetweenPages) + m_pageOffsets[offsetIndex].transform.localPosition;
                
                m_missions.Add(mission, new MissionInfo()
                {
                    m_worldInstance = missionInstance,
                    m_mission = mission,
                    m_status = MissionStatus.Ready
                });

                missionWorldIndex++;
            }

            ServiceLocator.Instance.TimeManager.Day.OnChangedValues += OnDayChanged;
        }

        private void OnDayChanged(int _, int day)
        {
            foreach (var (mission, missionInfo) in m_missions)
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
            var party = ServiceLocator.Instance.MonsterManager.GetParty(mission);

            int terrorReduction = GetAggregatePartyStatValue(SchemaStat.Stat.Terror, party) /
                                  m_gameSettings.MissionSpeedTerrorPerDay;

            // The minimum amount of time a mission can take is 0 days
            int endStep = startStep + Math.Max(0, mission.Days - terrorReduction);
            
            // TODO: Simulate the combat and generate a success ratio from 0f-1f
            float score = 1.0f;

            return (endStep, score);
        }

        private int GetAggregatePartyStatValue(SchemaStat.Stat stat, MonsterManager.MonsterInfo[] party)
        {
            int value = 0;
            foreach (var monsterInfo in party)
            {
                if (monsterInfo == null)
                {
                    continue;
                }
                
                value += monsterInfo.m_worldInstance.GetStatValue(stat);
            }
            return value;
        }

        #endregion
    }
}
