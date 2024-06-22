using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIMonsterRecruit : SerializedMonoBehaviour, IWorldInstanceController<Monster>
    {

        [SerializeField] private UIMonsterDetails m_monsterDetails;
        
        public void SetInstance(Monster data)
        {
            m_monsterDetails.SetInstance(data);
        }
    }
}
