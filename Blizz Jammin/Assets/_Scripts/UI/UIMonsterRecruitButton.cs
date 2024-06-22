using _Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMonsterRecruitButton : MonoBehaviour, IWorldInstanceController<Monster>
    {
        [SerializeField] private Button m_button;
        
        [SerializeField] private UIMonsterDetails m_monsterDetails;
        
        // TODO: Badging state
        [SerializeField] private GameObject m_badge;

        public Button Button => m_button;

        private Monster m_instance;
        
        public void SetInstance(Monster instance)
        {
            m_instance = instance;
            m_monsterDetails.SetInstance(instance);
            UpdateBadge();
        }

        public void UpdateBadge()
        {
            m_badge.SetActive(m_instance != null && m_instance.IsNew);
        }
    }
}
