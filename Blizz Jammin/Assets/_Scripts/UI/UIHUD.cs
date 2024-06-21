using _Scripts.Gameplay;
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
        
        private void Awake()
        {
            m_timerText.SetText(string.Format(c_timerFormat, 0));
            ServiceLocator.Instance.TimeManager.Day.OnChangedValues += OnDayChanged;
            
            m_infamyText.SetText(string.Format(c_infamyFormat, 0));
            ServiceLocator.Instance.RecruitManager.Infamy.OnChangedValues += OnInfamyChanged;
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
