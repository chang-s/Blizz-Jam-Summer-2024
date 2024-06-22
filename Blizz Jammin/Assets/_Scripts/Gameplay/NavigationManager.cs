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
        public Vector3 HorizontalOffset;
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

        private bool m_showingMonsters;
        private Vector3 m_cameraStart;
        private int m_maxMonsters;
        private int m_maxMissions;
        private int m_monsterPage;
        private int m_missionPage;
        private int m_maxMonsterPages;
        private int m_maxMissionPages;
        private float m_animationTimeRemaining;
        
        private void Awake()
        {
            m_showingMonsters = true;
            m_monsterPage = m_missionPage = 0;
            m_maxMonsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters().Count;
            m_maxMissions = ServiceLocator.Instance.AllMissions.Length;
            m_maxMonsterPages = m_maxMonsters / EntriesPerPage + (m_maxMonsters % EntriesPerPage == 0 ? 0 : 1) - 1;
            m_maxMissionPages = m_maxMissions / EntriesPerPage + (m_maxMissions % EntriesPerPage == 0 ? 0 : 1) - 1;

            UpdateNavButtons();
            
            Lair.onClick.AddListener(OnLairButtonClicked);
            Missions.onClick.AddListener(OnMissionButtonClicked);
            Left.onClick.AddListener(OnLeftButtonClicked);
            Right.onClick.AddListener(OnRightButtonClicked);

            ServiceLocator.Instance.MonsterManager.OnMonsterRecruited += OnMonsterRecruited;
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

            if (m_showingMonsters)
            {
                m_monsterPage++;
            }
            else
            {
                m_missionPage++;
            }

            Camera.transform.DOMove(
                Camera.transform.position + HorizontalOffset,
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
            
            if (m_showingMonsters)
            {
                m_monsterPage--;
            }
            else
            {
                m_missionPage--;
            }
            
            Camera.transform.DOMove(
                Camera.transform.position - HorizontalOffset,
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

            int pages = m_missionPage - m_monsterPage;
            Vector3 horizontalOffset = pages * HorizontalOffset;
            
            Camera.transform.DOMove(
                Camera.transform.position - VerticalOffset + horizontalOffset,
                Duration
            ).SetEase(Ease);
            
            m_showingMonsters = false;
            UpdateNavButtons();
        }

        private void OnLairButtonClicked()
        {
            if (IsAnimating())
            {
                return;
            }

            m_animationTimeRemaining = Duration;
            
            int pages = m_monsterPage - m_missionPage;
            Vector3 horizontalOffset = pages * HorizontalOffset;

            Camera.transform.DOMove(
                Camera.transform.position + VerticalOffset + horizontalOffset,
                Duration
            ).SetEase(Ease);

            m_showingMonsters = true;
            UpdateNavButtons();
        }

        private void OnMonsterRecruited(Monster _)
        {
            m_maxMonsters = ServiceLocator.Instance.MonsterManager.GetOwnedMonsters().Count;
            m_maxMonsterPages = m_maxMonsters / EntriesPerPage + (m_maxMonsters % EntriesPerPage == 0 ? 0 : 1) - 1;
            UpdateNavButtons();
        }
        
        private void UpdateNavButtons()
        {
            Lair.gameObject.SetActive(!m_showingMonsters);
            Missions.gameObject.SetActive(m_showingMonsters);

            bool showLeft = m_showingMonsters ? m_monsterPage > 0 : m_missionPage > 0;
            Left.gameObject.SetActive(showLeft);

            bool showRight = m_showingMonsters ? m_monsterPage < m_maxMonsterPages : m_missionPage < m_maxMissionPages;
            Right.gameObject.SetActive(showRight);
        }
    }
}
