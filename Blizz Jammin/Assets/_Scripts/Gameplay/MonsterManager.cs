using System.Collections.Generic;
using _Scripts.Schemas;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Gameplay
{
    public class MonsterManager : MonoBehaviour
    {
        [SerializeField] private SchemaMonster[] m_startingMonsters;
        [SerializeField] private Monster m_monsterPrefab;
        [SerializeField] private Transform m_monsterRoot;

        private List<Monster> m_monsters = new List<Monster>();
        
        private void Awake()
        {
            foreach (var monsterSchema in m_startingMonsters)
            {
                var randomLocationInNavMesh = GetRandomNavMeshLocation(7.5f);
                Monster monster = Instantiate(m_monsterPrefab, randomLocationInNavMesh, Quaternion.identity);
                monster.transform.SetParent(m_monsterRoot);
                monster.SetData(monsterSchema);
                m_monsters.Add(monster);
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

        // TODO: protect this
        public Monster GetMonster(int index)
        {
            return m_monsters[index];
        }
    }
}
