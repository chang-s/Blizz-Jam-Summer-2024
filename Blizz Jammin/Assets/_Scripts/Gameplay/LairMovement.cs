using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Scripts.Gameplay
{
    public class LairMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent m_agent;

        private Stopwatch m_stopwatch = new Stopwatch();
        private float m_periodMs;

        private void Start()
        {
            m_stopwatch.Start();
            m_periodMs = Random.Range(1000, 3000);
        }

        private void Update()
        {
            if (m_stopwatch.ElapsedMilliseconds < m_periodMs)
            {
                return;
            }
            
            m_stopwatch.Restart();
            m_periodMs = Random.Range(1000, 3000);
            Vector3 randomNavPosition = GetRandomNavMeshLocation(2f);
            m_agent.SetDestination(randomNavPosition);
        }
        
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
    }
}
