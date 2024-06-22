using _Scripts.Gameplay;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMonsterRecruit : SerializedMonoBehaviour, IWorldInstanceController<Monster>
    {
        [BoxGroup("Popup")] 
        [SerializeField] private UIPopup m_popup;
        
        /// <summary>
        /// This handles all the details/stats for the monster we need to show, via SetInstance.
        /// </summary>
        [BoxGroup("Shown Monster")]
        [SerializeField] private UIMonsterDetails m_monsterDetails;

        [BoxGroup("Shop")] 
        [SerializeField] private GameObject m_outOfStock;
        
        [BoxGroup("Shop")] 
        [SerializeField] private TMP_Text m_costLabel;
        
        [BoxGroup("Shop")] 
        [SerializeField] private Button m_recruitButton;
        
        [BoxGroup("Shop")] 
        [SerializeField] private UIMonsterRecruitButton[] m_monsterButtons;
        
        [BoxGroup("Shop")] 
        [SerializeField] private Button m_left;
        
        [BoxGroup("Shop")] 
        [SerializeField] private Button m_right;

        private Monster m_shownMonster = null;
        private int m_shownMonsterIndex = -1;
        
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

            for (var i = 0; i < m_monsterButtons.Length; i++)
            {
                var capturedIndex = i;
                m_monsterButtons[i].Button.onClick.AddListener(() =>
                {
                    OnMonsterButtonClicked(capturedIndex);
                });
            }

            m_right.onClick.AddListener(GoToNextMonster);
            m_left.onClick.AddListener(GoToPreviousMonster);

            UpdateMonsterButtons();
            UpdateRecruitButtonState();
            
            // Start by looking at the first monster
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);
            if (monsters.Count > 0)
            {
                m_shownMonsterIndex = 0;
                m_monsterButtons[m_shownMonsterIndex].Button.Select();
                SetInstance(monsters[0]);
            }

            m_popup.OnShow += OnPopupShown;

            ServiceLocator.Instance.MonsterManager.OnMonsterUnlocked += HandleMonsterUpdate;
            ServiceLocator.Instance.MonsterManager.OnMonsterRecruited += HandleMonsterUpdate;
        }

        private void HandleMonsterUpdate(Monster _)
        {
            UpdateMonsterButtons();
            
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);
            if (monsters.Count > 0)
            {
                m_shownMonsterIndex = 0;
                m_monsterButtons[m_shownMonsterIndex].Button.Select();
                SetInstance(monsters[0]);
            }
        }

        private void OnPopupShown()
        {
            // We need to re-select the last thing clicked, because clicking on anything will deselect it
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);
            if (m_shownMonsterIndex < monsters.Count)
            {
                m_monsterButtons[m_shownMonsterIndex].Button.Select();
            }
        }

        private void GoToNextMonster()
        {
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);
            m_shownMonsterIndex++;
            if (m_shownMonsterIndex >= monsters.Count)
            {
                m_shownMonsterIndex = 0;
            }
            m_monsterButtons[m_shownMonsterIndex].Button.Select();
            SetInstance(monsters[m_shownMonsterIndex]);
        }
        
        private void GoToPreviousMonster()
        {
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);
            m_shownMonsterIndex--;
            if (m_shownMonsterIndex < 0)
            {
                m_shownMonsterIndex = monsters.Count - 1;
            }
            m_monsterButtons[m_shownMonsterIndex].Button.Select();
            SetInstance(monsters[m_shownMonsterIndex]);
        }

        private void OnMonsterButtonClicked(int buttonIndex)
        {
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);
            if (buttonIndex >= monsters.Count)
            {
                return;
            }

            m_shownMonsterIndex = buttonIndex;
            SetInstance(monsters[buttonIndex]);
        }

        private void UpdateMonsterButtons()
        {
            // The UI only supports a specific amount of buttons, so we must clamp the monsters, even if there
            // are more to recruit, we will only show as many as we can
            var monsters = ServiceLocator.Instance.MonsterManager.GetMonsters(Monster.MonsterStatus.Purchasable);

            if (monsters.Count == 0)
            {
                m_outOfStock.SetActive(true);
                return;
            }
            m_outOfStock.SetActive(false);
            
            for (var i = 0; i < m_monsterButtons.Length; i++)
            {
                // No more to show, we can hide these buttons
                var button = m_monsterButtons[i];
                if (i >= monsters.Count)
                {
                    button.gameObject.SetActive(false);
                    continue;
                }
                
                button.gameObject.SetActive(true);
                button.SetInstance(monsters[i]);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
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
            ServiceLocator.Instance.MonsterManager.Recruit(m_shownMonster);

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
