using System.Diagnostics;
using UnityEngine;
using Utility.Observable;

namespace _Scripts.Gameplay
{
    public class TimeManager : MonoBehaviour
    {
        // TODO: Make this a scriptable object
        [SerializeField] 
        private float m_secondsPerDay = 1f;

        [HideInInspector]
        public Observable<int> Day = new Observable<int>(0);

        private Stopwatch m_timer = new Stopwatch();

        private void Awake()
        {
            ServiceLocator.Instance.MissionManager.OnMissionStatusChanged += OnMissionStatusChanged;
        }

        private void OnMissionStatusChanged(MissionManager.MissionInfo changedMissionInfo)
        {
            bool anyMissionInCombat = ServiceLocator.Instance.MissionManager.IsAnyMissionInCombat();
            if (anyMissionInCombat)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }

        private void Update()
        {
            int day = (int)((m_timer.ElapsedMilliseconds / 1000) / m_secondsPerDay);
            if (day != Day.Value)
            {
                Day.Value = day;
            }
        }

        public void Pause()
        {
            m_timer.Stop();
        }

        public void Continue()
        {
            m_timer.Start();
        }
    }
}
