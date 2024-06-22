using System.Linq;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIHUD : MonoBehaviour
    {
        private const string c_timerFormat = "Day {0}";
        private const string c_infamyFormat = "{0}";
        
        [BoxGroup("Panel")]
        [SerializeField] private TMP_Text m_timerText;

        [BoxGroup("Panel")]
        [SerializeField] private TMP_Text m_infamyText;
        
        [BoxGroup("Badges")]
        [SerializeField] private GameObject m_badgeMission;
        [BoxGroup("Badges")]
        [SerializeField] private GameObject m_badgeRecruit;
        
        private void Awake()
        {
            m_timerText.SetText(string.Format(c_timerFormat, 0));
            ServiceLocator.Instance.TimeManager.Day.OnChangedValues += OnDayChanged;
            
            m_infamyText.SetText(string.Format(c_infamyFormat, 0));
            ServiceLocator.Instance.Infamy.OnChangedValues += OnInfamyChanged;

            ServiceLocator.Instance.MonsterManager.OnMonsterUnlocked += OnMonsterUnlocked;
            ServiceLocator.Instance.MissionManager.OnMissionStatusChanged += OnMissionStatusChanged;
            
            var monsterRecruitPopup = ServiceLocator.Instance.UIPopupManager.GetPopup(SchemaPopup.PopupType.MonsterRecruit);
            monsterRecruitPopup.OnHide += HandleRecruitBadge;
            HandleRecruitBadge();
        }

        private void OnMonsterUnlocked(Monster _)
        {
            HandleRecruitBadge();
        }

        private void HandleRecruitBadge()
        {
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);
            int newMonsters = monsters.Count(m => m.IsNew);
            m_badgeRecruit.SetActive(newMonsters > 0);
        }
        
        private void OnMissionStatusChanged(MissionManager.MissionInfo _)
        {
            int rewardsToClaim = ServiceLocator.Instance.MissionManager.GetUnclaimedRewardCount();
            m_badgeMission.SetActive(rewardsToClaim > 0);
        }

        private void OnDayChanged(int oldValue, int newValue)
        {
            m_timerText.SetText(string.Format(c_timerFormat, newValue));
        }
        
        private void OnInfamyChanged(int oldValue, int newValue)
        {
            m_infamyText.SetText(string.Format(c_infamyFormat, newValue));
        }

    }
}
