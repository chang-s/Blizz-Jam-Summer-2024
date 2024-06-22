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
        
        public void SetInstance(Monster instance)
        {
            m_monsterDetails.SetInstance(instance);
        }
    }
}
