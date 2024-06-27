using TMPro;
using UnityEngine;

namespace _Scripts.UI.Tooltip
{
    public class UITooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_label;

        private void Update()
        {
            transform.position = Input.mousePosition;
        }
        
        public void Show(string toDisplay)
        {
            // TODO: Tween it
            gameObject.SetActive(true);
            m_label.SetText(toDisplay);
        }

        public void Hide()
        {
            // TODO: Tween it
            gameObject.SetActive(false);
        }
    }
}
