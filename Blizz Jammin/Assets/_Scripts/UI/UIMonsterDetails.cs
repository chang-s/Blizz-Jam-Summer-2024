using _Scripts.Gameplay;
using UnityEngine;

namespace _Scripts.UI
{
    [RequireComponent(typeof(UIPopup))]
    public class UIMonsterDetails : MonoBehaviour
    {
        [SerializeField] private UIPopup m_popup;

        public void ShowDetails(Monster monster)
        {
            
        }
    }
}
