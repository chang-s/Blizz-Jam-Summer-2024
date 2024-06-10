using System;
using System.Collections.Generic;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    //  TODO: It might behoove us to split this into MonsterManager and PartyManager, but for now we are getting away with 1 class
    public class MonsterManager : MonoBehaviour
    {
        private readonly MonsterInfo[] c_emptyParty = new MonsterInfo[] { };

        public enum MonsterStatus
        {
            Locked,             // Cannot purchase yet
            Purchasable,        // Is in the shop
            Purchased,          // Is bought and owned
            Busy                // Is owned, but is currently busy in combat or otherwise
        }

        public class MonsterInfo
        {
            public Monster m_worldInstance;
            public MonsterStatus m_status;
            public SchemaMission m_currentMission;
        }

        [BoxGroup("Lair")]
        [SerializeField] private Monster m_monsterPrefab;
        [BoxGroup("Lair")]
        [SerializeField] private Transform m_monsterRoot;
        [BoxGroup("Lair")] [SerializeField] 
        private Vector3 m_gap;

        /// <summary>
        /// Invoked when a party has changed. Event has the mission for the party that has changed.
        /// </summary>
        public Action<SchemaMission> OnPartyChanged;
        
        /// <summary>
        /// Internal tracking of all monsters in the game.
        /// </summary>
        private List<MonsterInfo> m_monsterInfos;
        
        /// <summary>
        /// Tracker of a collection of monsters per mission.
        /// </summary>
        private Dictionary<SchemaMission, MonsterInfo[]> m_parties = new Dictionary<SchemaMission, MonsterInfo[]>(); 

        private void Awake()
        {
            Vector3 offset = Vector3.zero;
            m_monsterInfos = new List<MonsterInfo>();
            foreach (var monsterSchema in ServiceLocator.Instance.AllMonsters)
            {
                
                Monster monster = Instantiate(m_monsterPrefab, m_monsterRoot);
                monster.transform.localPosition += offset;
                monster.SetData(monsterSchema);
                
                m_monsterInfos.Add(new MonsterInfo()
                {
                    m_worldInstance = monster,
                    m_status = monsterSchema.IsStarter ? MonsterStatus.Purchased : MonsterStatus.Locked
                });

                offset += m_gap;
            }
            
            foreach (var mission in ServiceLocator.Instance.AllMissions)
            {
                m_parties.Add(mission, new MonsterInfo[mission.MaxCapacity]);
            }
        }
        
        public List<MonsterInfo> GetOwnedMonsters()
        {
            List<MonsterInfo> results = new List<MonsterInfo>();
            foreach (MonsterInfo info in m_monsterInfos)
            {
                if (info.m_status > MonsterStatus.Purchasable)
                {
                    results.Add(info);
                }
            }

            return results;
        }
        
        public MonsterInfo[] GetParty(SchemaMission mission)
        {
            if (!m_parties.ContainsKey(mission))
            {
                return c_emptyParty;
            }
            
            return m_parties[mission];
        }

        public bool AddMonsterToParty(SchemaMonster monster, SchemaMission mission, int partyIndex)
        {
            if (!m_parties.ContainsKey(mission))
            {
                return false;
            }
            
            var party = m_parties[mission];
            if (party.Length <= partyIndex)
            {
                return false;
            }
            
            MonsterInfo monsterInfo = m_monsterInfos.Find(m => m.m_worldInstance.Data == monster);
            if (monsterInfo == null)
            {
                return false;
            }

            switch (monsterInfo.m_status)
            {
                case MonsterStatus.Locked or MonsterStatus.Purchasable:
                    return false;
                case MonsterStatus.Busy:
                    // If we try to equip a monster who is busy in another mission, do not allow it.
                    if (monsterInfo.m_currentMission != mission)
                    {
                        return false;
                    }
                    break;
            }
            
            // If this monster was already in this mission's party, clear it out first
            for (var i = 0; i < party.Length; i++)
            {
                if (party[i] != null && party[i] == monsterInfo)
                {
                    party[i] = null;
                }
            }

            // If there is a monster in this slot, untrack it (aka remove)
            if (party[partyIndex] != null)
            {
                party[partyIndex].m_status = MonsterStatus.Purchased;
                party[partyIndex].m_currentMission = null;
            }
            
            monsterInfo.m_status = MonsterStatus.Busy;
            monsterInfo.m_currentMission = mission;
            party[partyIndex] = monsterInfo;
            
            OnPartyChanged?.Invoke(mission);
            
            return true;
        }

        public bool RemoveMonsterFromParty(SchemaMonster monster, SchemaMission mission, int partyIndex)
        {
            if (!m_parties.ContainsKey(mission))
            {
                return false;
            }

            var party = m_parties[mission];
            if (party.Length <= partyIndex)
            {
                return false;
            }
            
            MonsterInfo monsterInfo = m_monsterInfos.Find(m => m.m_worldInstance.Data == monster);
            if (monsterInfo == null)
            {
                return false;
            }

            switch (monsterInfo.m_status)
            {
                case MonsterStatus.Locked or MonsterStatus.Purchasable:
                    return false;
            }
            
            monsterInfo.m_status = MonsterStatus.Purchased;
            monsterInfo.m_currentMission = null;
            party[partyIndex] = null;
            
            OnPartyChanged?.Invoke(mission);
            
            return true;
        }
    }
}
