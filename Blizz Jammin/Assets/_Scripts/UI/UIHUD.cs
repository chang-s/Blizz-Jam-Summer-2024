using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIHUD : MonoBehaviour
    {
        private const string c_timerFormat = "Day {0}";
        
        [BoxGroup("Timer")]
        [SerializeField] private TMP_Text m_timerText;

        private void Awake()
        {
            m_timerText.SetText(string.Format(c_timerFormat, 0));
            ServiceLocator.Instance.TimeManager.Day.OnChangedValues += OnDayChanged;
        }

        private void OnDayChanged(int oldValue, int newValue)
        {
            m_timerText.SetText(string.Format(c_timerFormat, newValue));
        }
    }
}
