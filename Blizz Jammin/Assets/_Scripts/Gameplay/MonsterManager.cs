using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility.Observable;

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
        [BoxGroup("Lair")]
        [SerializeField] private Transform m_waitingRoomRoot;
        [BoxGroup("Lair")] 
        [SerializeField] private Vector3 m_gap;
        [BoxGroup("Lair")] 
        [SerializeField] private int m_monstersPerPage;
        [BoxGroup("Lair")] 
        [SerializeField] Vector3 m_pageGap;
        
        /// <summary>
        /// Invoked when a monster is unlocked.
        /// </summary>
        public Action<Monster> OnMonsterUnlocked;
        
        /// <summary>
        /// Invoked when a monster is recruited.
        /// </summary>
        public Action<Monster> OnMonsterRecruited;
        
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
            // Make all the monster instances at startup
            m_monsters = new List<Monster>();
            foreach (var monsterSchema in ServiceLocator.Instance.AllMonsters)
            {
                Monster monster = Instantiate(m_monsterPrefab, m_waitingRoomRoot);
                monster.SetData(monsterSchema);

                if (monsterSchema.StartStatus >= Monster.MonsterStatus.Purchasable)
                {
                    monster.Unlock();
                }
                
                if (monsterSchema.StartStatus >= Monster.MonsterStatus.Ready)
                {
                    monster.Recruit();
                }
                
                m_monsters.Add(monster);
            }

            // Position the monsters in their starting spots
            PositionMonsters();
            
            // Make parties for all the missions
            foreach (var mission in ServiceLocator.Instance.AllMissions)
            {
                m_parties.Add(mission, new Monster[mission.MaxCapacity]);
            }
            ServiceLocator.Instance.MissionManager.OnMissionStatusChanged += OnMissionStatusChanged;
        }

        public void Unlock(Monster monster)
        {
            if (monster.Status > Monster.MonsterStatus.Locked)
            {
                return;
            }
            
            monster.Unlock();
            OnMonsterUnlocked?.Invoke(monster);
        }

        public void Recruit(Monster monster)
        {
            // We must unlock it forcibly if it isn't to recruit it
            Unlock(monster);
            
            monster.Recruit();
            
            // Move this new monster to the front of the list
            int oldIndex = m_monsters.FindIndex(m => m == monster);
            Monster firstMonster = m_monsters[0];
            m_monsters[0] = monster;
            m_monsters[oldIndex] = firstMonster;
            
            PositionMonsters();

            ServiceLocator.Instance.SoundManager.RequestSfx(monster.Data.RecruitSfx);
            
            OnMonsterRecruited?.Invoke(monster);
        }
        
        /// <summary>
        /// This function assumes that m_monsters is filled out. It will place the monsters from start -> end,
        /// in the order that they are listed. If a monster is NOT purchased, then it will be skipped!
        /// </summary>
        private void PositionMonsters()
        {
            Vector3 offset = Vector3.zero;
            int monsterCount = 0;
            for (var i = 0; i < m_monsters.Count; i++)
            {
                var monster = m_monsters[i];
                if (monster.Status < Monster.MonsterStatus.Ready)
                {
                    continue;
                }

                monsterCount++;
                m_monsters[i].transform.SetParent(m_monsterRoot);
                m_monsters[i].transform.localPosition = offset;
                
                offset += m_gap;
                if (monsterCount % m_monstersPerPage == 0)
                {
                    offset =  m_pageGap * monsterCount / m_monstersPerPage;
                }
            }
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

        public List<Monster> GetMonsters(Monster.MonsterStatus status)
        {
            List<Monster> results = new List<Monster>();
            foreach (Monster info in m_monsters)
            {
                if (info.Status == status)
                {
                    results.Add(info);
                }
            }

            return results;
        }

        public List<Monster> GetOwnedMonsters()
        {
            var monsters= GetMonsters(Monster.MonsterStatus.Ready);
            monsters.AddRange(GetMonsters(Monster.MonsterStatus.Busy));
            return monsters;
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
