using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay
{
    public class MissionPage : MonoBehaviour
    {
        [BoxGroup("Location")]
        [SerializeField] 
        private Transform[] m_missionLocations;
    }
}
