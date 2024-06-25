using _Scripts.Gameplay;
using _Scripts.Gameplay.Instances;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UILootEquipEntry : SerializedMonoBehaviour, IInstanceController<InstanceLoot>
    {
        public Button Button => m_button;
        
        [SerializeField] private UILoot m_uiLoot;
        [SerializeField] private Button m_button;
            
        private InstanceLoot m_loot;
        
        public void SetInstance(InstanceLoot instance)
        {
            m_loot = instance;
            m_uiLoot.SetInstance(m_loot);
        }
    }
}
