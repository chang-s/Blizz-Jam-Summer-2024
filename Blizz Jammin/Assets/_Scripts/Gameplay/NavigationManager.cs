using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay
{
    // TODO: There is a defect with the current setup/logic
    // This will go up, down, left and right, but that means when you go left/right during MISSIONS, this affects
    // the position when going back to MONSTERS
    // We need to lerp back to the last page that monsters were at, but this will mean a weird movement
    // We might want to consider doing 2 cameras an then doing a fade instead of translating up/down
    public class NavigationManager : SerializedMonoBehaviour
    {
        [BoxGroup("Camera Movement")]
        public Camera Camera;
        [BoxGroup("Camera Movement")]
        public Vector3 LairHorizontalOffset;
        [BoxGroup("Camera Movement")]
        public Vector3 MissionHorizontalOffset;
        [BoxGroup("Camera Movement")]
        public Vector3 VerticalOffset;
        [BoxGroup("Camera Movement")]
        public int EntriesPerPage;

        
        [BoxGroup("Animation")]
        public Ease Ease;
        [BoxGroup("Animation")]
        public float Duration;
        
        [BoxGroup("Buttons")]
        public Button Left;
        [BoxGroup("Buttons")]
        public Button Right;
        [BoxGroup("Buttons")]
        public Button Lair;
        [BoxGroup("Buttons")]
        public Button Missions;

        private bool m_showingLair;
        private Vector3 m_cameraStart;
        private int m_maxMonsters;
        private int m_maxMissions;
        private int m_page;
        private int m_maxMonsterPages;
        private int m_maxMissionPages;
        private float m_animationTimeRemaining;
        
        private void Awake()
        {
            m_showingLair = true;
            m_page = 0;
            m_maxMonsters = ServiceLocator.Instance.AllMonsters.Length;
            m_maxMissions = ServiceLocator.Instance.AllMissions.Length;
            m_maxMonsterPages = m_maxMonsters / EntriesPerPage;
            m_maxMissionPages = m_maxMissions / EntriesPerPage;

            UpdateNavButtons();
            
            Lair.onClick.AddListener(OnLairButtonClicked);
            Missions.onClick.AddListener(OnMissionButtonClicked);
            Left.onClick.AddListener(OnLeftButtonClicked);
            Right.onClick.AddListener(OnRightButtonClicked);
        }

        private void Update()
        {
            if (!IsAnimating())
            {
                return;
            }
            
            m_animationTimeRemaining -= Time.deltaTime;
        }

        private bool IsAnimating()
        {
            return m_animationTimeRemaining > 0;
        }

        private void OnRightButtonClicked()
        {
            if (IsAnimating())
            {
                return;
            }

            m_animationTimeRemaining = Duration;
            
            m_page++;

            var offset = m_showingLair ? LairHorizontalOffset : MissionHorizontalOffset;
            Camera.transform.DOMove(
                Camera.transform.position + offset,
                Duration
            ).SetEase(Ease);
            
            UpdateNavButtons();
        }

        private void OnLeftButtonClicked()
        {
            if (IsAnimating())
            {
                return;
            }

            m_animationTimeRemaining = Duration;
            
            m_page--;

            var offset = m_showingLair ? LairHorizontalOffset : MissionHorizontalOffset;
            Camera.transform.DOMove(
                Camera.transform.position - offset,
                Duration
            ).SetEase(Ease);
            
            UpdateNavButtons();
        }

        private void OnMissionButtonClicked()
        {
            if (IsAnimating())
            {
                return;
            }

            m_animationTimeRemaining = Duration;
            
            Camera.transform.DOMove(
                Camera.transform.position - VerticalOffset,
                Duration
            ).SetEase(Ease);
            
            m_showingLair = false;
            UpdateNavButtons();
        }

        private void OnLairButtonClicked()
        {
            if (IsAnimating())
            {
                return;
            }

            m_animationTimeRemaining = Duration;
            
            Camera.transform.DOMove(
                Camera.transform.position + VerticalOffset,
                Duration
            ).SetEase(Ease);

            m_showingLair = true;
            UpdateNavButtons();
        }

        private void UpdateNavButtons()
        {
            Lair.gameObject.SetActive(!m_showingLair);
            Missions.gameObject.SetActive(m_showingLair);

            bool showLeft = m_page > 0;
            Left.gameObject.SetActive(showLeft);

            bool showRight = m_showingLair ? m_page < m_maxMonsterPages : m_page < m_maxMissionPages;
            Right.gameObject.SetActive(showRight);
        }
    }
}
