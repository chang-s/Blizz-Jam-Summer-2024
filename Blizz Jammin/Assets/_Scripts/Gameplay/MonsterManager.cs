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
        private readonly Monster[] c_emptyParty = new Monster[] { };
        
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
        private List<Monster> m_monsters;
        
        /// <summary>
        /// Tracker of a collection of monsters per mission.
        /// </summary>
        private Dictionary<SchemaMission, Monster[]> m_parties = new Dictionary<SchemaMission, Monster[]>(); 

        private void Awake()
        {
            Vector3 offset = Vector3.zero;
            m_monsters = new List<Monster>();
            foreach (var monsterSchema in ServiceLocator.Instance.AllMonsters)
            {
                Monster monster = Instantiate(m_monsterPrefab, m_monsterRoot);
                monster.transform.localPosition += offset;
                monster.SetData(monsterSchema);

                // All "starter" monsters are unlocked and recruited at the start of the game
                if (monsterSchema.IsStarter)
                {
                    monster.Unlock();
                    monster.Recruit();
                }
                
                m_monsters.Add(monster);
                
                offset += m_gap;
            }
            
            foreach (var mission in ServiceLocator.Instance.AllMissions)
            {
                m_parties.Add(mission, new Monster[mission.MaxCapacity]);
            }

            ServiceLocator.Instance.MissionManager.OnMissionStatusChanged += OnMissionStatusChanged;
        }

        private void OnMissionStatusChanged(MissionManager.MissionInfo missionInfo)
        {
            // When a mission becomes ready, we should de-associate all monsters in the party
            if (missionInfo.m_status == MissionManager.MissionStatus.Ready)
            {
                var party = GetParty(missionInfo.m_mission.Data);
                for (var i = 0; i < party.Length; i++)
                {
                    var partyMember = party[i];
                    if (partyMember == null)
                    {
                        continue;
                    }

                    // TODO: Clean up this call/usage/storage of SchemaMonster/SchemaMission
                    RemoveMonsterFromParty(partyMember.Data, missionInfo.m_mission.Data, i);
                }
            }
        }

        public List<Monster> GetOwnedMonsters()
        {
            List<Monster> results = new List<Monster>();
            foreach (Monster info in m_monsters)
            {
                if (info.Status > Monster.MonsterStatus.Purchasable)
                {
                    results.Add(info);
                }
            }

            return results;
        }
        
        public Monster[] GetParty(SchemaMission mission)
        {
            if (!m_parties.ContainsKey(mission))
            {
                return c_emptyParty;
            }
            
            return m_parties[mission];
        }
        
        public bool AddMonsterToParty(SchemaMonster schema, SchemaMission mission, int partyIndex)
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
            
            Monster monster = m_monsters.Find(m => m.Data == schema);
            if (monster == null)
            {
                return false;
            }

            switch (monster.Status)
            {
                case Monster.MonsterStatus.Locked or Monster.MonsterStatus.Purchasable:
                    return false;
                case Monster.MonsterStatus.Busy:
                    // If we try to equip a schema who is busy in another mission, do not allow it.
                    if (monster.CurrentMission != mission)
                    {
                        return false;
                    }
                    break;
            }
            
            // If this schema was already in this mission's party, clear it out first
            for (var i = 0; i < party.Length; i++)
            {
                if (party[i] != null && party[i] == monster)
                {
                    party[i] = null;
                }
            }
            
            if (party[partyIndex] != null)
            {
                RemoveMonsterFromParty(party[partyIndex].Data, mission, partyIndex);
            }
            
            monster.BeginMission(mission);
            party[partyIndex] = monster;
            
            OnPartyChanged?.Invoke(mission);
            
            return true;
        }

        public bool RemoveMonsterFromParty(SchemaMonster schema, SchemaMission mission, int partyIndex)
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
            
            Monster monster = m_monsters.Find(m => m.Data == schema);
            if (monster == null)
            {
                return false;
            }

            switch (monster.Status)
            {
                case Monster.MonsterStatus.Locked or Monster.MonsterStatus.Purchasable:
                    return false;
            }
            
            monster.EndMission(mission);
            party[partyIndex] = null;
            
            OnPartyChanged?.Invoke(mission);
            
            return true;
        }
    }
}
