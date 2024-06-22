using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMonsterRecruit : SerializedMonoBehaviour, IWorldInstanceController<Monster>
    {
        /// <summary>
        /// This handles all the details/stats for the monster we need to show, via SetInstance.
        /// </summary>
        [BoxGroup("Monster Details")]
        [SerializeField] private UIMonsterDetails m_monsterDetails;

        [BoxGroup("Shop Specific")] 
        [SerializeField] private TMP_Text m_costLabel;
        
        [BoxGroup("Shop Specific")] 
        [SerializeField] private Button m_recruitButton;

        private Monster m_shownMonster = null;
        
        public void SetInstance(Monster instance)
        {
            m_shownMonster = instance;
            
            // Feed the monster details
            m_monsterDetails.SetInstance(instance);
            
            // Update the cost label
            m_costLabel.SetText(instance.Data.Cost.ToString());

            UpdateRecruitButtonState();
        }

        private void Awake()
        {
            m_recruitButton.onClick.AddListener(OnRecruitButtonClicked);
            ServiceLocator.Instance.Infamy.OnChanged += UpdateRecruitButtonState;
            
            UpdateRecruitButtonState();
        }

        private void OnRecruitButtonClicked()
        {
            // No monster being shown, do nothing
            if (m_shownMonster == null)
            {
                return;
            }

            // Cannot afford the monster
            var cost = m_shownMonster.Data.Cost;
            if (ServiceLocator.Instance.Infamy.Value < cost)
            {
                return;
            }

            ServiceLocator.Instance.DeltaInfamy(-cost);
            m_shownMonster.Unlock();
            m_shownMonster.Recruit();

            // TODO: Pan to the new monster!
            
            // Close the recruit popup
            ServiceLocator.Instance.UIPopupManager.RequestClose();
        }

        private void UpdateRecruitButtonState()
        {
            bool canRecruit = true;
            if (m_shownMonster == null)
            {
                canRecruit = false;
            }
            else
            {
                canRecruit = m_shownMonster.Data.Cost <= ServiceLocator.Instance.Infamy.Value;
            }

            m_costLabel.color = canRecruit ? Color.black : Color.red;
            m_recruitButton.interactable = canRecruit;
        }
    }
}
