using System;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMissionResults : MonoBehaviour, ISchemaController<SchemaMission>
    {
        [SerializeField] private Button m_accept;
        
        private SchemaMission m_mission;
        
        public void SetData(SchemaMission data)
        {
            m_mission = data;
            // TODO:
        }

        private void Awake()
        {
            m_accept.onClick.AddListener(OnAcceptButtonClicked);
        }

        private void OnAcceptButtonClicked()
        {
            // TODO: Grant/Celebrate Rewards
            ServiceLocator.Instance.MissionManager.ClaimRewards(m_mission);
        }
    }
}
