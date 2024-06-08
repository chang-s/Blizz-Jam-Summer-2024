using System.Collections.Generic;
using _Scripts.Schemas;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay
{
    public class MonsterManager : MonoBehaviour
    {
        private enum MonsterStatus
        {
            Locked,         // Cannot purchase yet
            Purchasable,      // Is in the shop
            Purchased,          // Is bought and owned
            Busy            // Is owned, but is currently busy in combat or otherwise
        }

        private struct MonsterInfo
        {
            public Monster m_worldInstance;
            public MonsterStatus m_status;
        }
        
        [SerializeField] private SchemaMonster[] m_startingMonsters;
        [SerializeField] private Monster m_monsterPrefab;
        [SerializeField] private Transform m_monsterRoot;

        private List<MonsterInfo> m_monsterInfos;
        
        private void Awake()
        {
            // TEMP: We own the starting monsters
            m_monsterInfos = new List<MonsterInfo>();
            foreach (var monsterSchema in m_startingMonsters)
            {
                var randomLocationInNavMesh = GetRandomNavMeshLocation(7.5f);
                Monster monster = Instantiate(m_monsterPrefab, randomLocationInNavMesh, Quaternion.identity);
                monster.transform.SetParent(m_monsterRoot);
                monster.SetData(monsterSchema);
                
                m_monsterInfos.Add(new MonsterInfo()
                {
                    m_worldInstance = monster,
                    m_status = MonsterStatus.Purchasable
                });
            }
        }
        
        // TODO: Write a Utility class/function for this
        private Vector3 GetRandomNavMeshLocation(float radius) {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
                finalPosition = hit.position;            
            }
            
            return finalPosition;
        }

        // TODO: TEMP
        public List<Monster> GetOwnedMonsters()
        {
            List<Monster> results = new List<Monster>();
            foreach (MonsterInfo info in m_monsterInfos)
            {
                if (info.m_status is MonsterStatus.Purchased or MonsterStatus.Busy)
                {
                    results.Add(info.m_worldInstance);
                }
            }

            return results;
        }
        
        public List<Monster> GetMonstersAvailableForMission()
        {
            List<Monster> results = new List<Monster>();
            foreach (MonsterInfo info in m_monsterInfos)
            {
                if (info.m_status == MonsterStatus.Purchased)
                {
                    results.Add(info.m_worldInstance);
                }
            }

            return results;
        }
    }
}
