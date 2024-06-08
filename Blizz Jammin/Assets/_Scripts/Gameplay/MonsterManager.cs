using System.Collections.Generic;
using _Scripts.Schemas;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay
{
    public class MonsterManager : MonoBehaviour
    {
        private enum Status
        {
            InShop,
            Owned,
            InCombat
        }
        
        [SerializeField] private SchemaMonster[] m_startingMonsters;
        [SerializeField] private Monster m_monsterPrefab;
        [SerializeField] private Transform m_monsterRoot;

        // TODO: Better system for tracking
        private Dictionary<Status, List<Monster>> m_monsters = new Dictionary<Status, List<Monster>>();
        
        private void Awake()
        {
            m_monsters.Add(Status.InShop, new List<Monster>());
            m_monsters.Add(Status.InCombat, new List<Monster>());

            // TEMP: We own the starting monsters
            var owned = new List<Monster>();
            foreach (var monsterSchema in m_startingMonsters)
            {
                var randomLocationInNavMesh = GetRandomNavMeshLocation(7.5f);
                Monster monster = Instantiate(m_monsterPrefab, randomLocationInNavMesh, Quaternion.identity);
                monster.transform.SetParent(m_monsterRoot);
                monster.SetData(monsterSchema);
                
                owned.Add(monster);
            }
            m_monsters.Add(Status.Owned, owned);
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
        [CanBeNull]
        public List<Monster> GetOwnedMonsters()
        {
            return m_monsters[Status.Owned];
        }
    }
}
