using System.Collections.Generic;
using Sirenix.OdinInspector;
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
        
        [BoxGroup("Lair")]
        [SerializeField] private Monster m_monsterPrefab;
        [BoxGroup("Lair")]
        [SerializeField] private Transform m_monsterRoot;
        [BoxGroup("Lair")] [SerializeField] 
        private Vector3 m_gap;
        
        private List<MonsterInfo> m_monsterInfos;
        
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
