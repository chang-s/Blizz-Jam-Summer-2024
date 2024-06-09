using System;
using System.Collections.Generic;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIPartyMonster : SerializedMonoBehaviour, ISchemaController<SchemaMonster>
    {
        [Serializable]
        public enum PartyState
        {
            Available,
            Selected,
            InParty,
            InCombat
        }
        
        [SerializeField] private UIMonsterDetails m_monsterDetails;
        [SerializeField] private Button m_button;
        [SerializeField] private Dictionary<PartyState, GameObject> m_stateControllers;
        
        public void SetData(SchemaMonster data)
        {
            m_monsterDetails.SetData(data);
        }

        public void SetState(PartyState state)
        {
            if (!m_stateControllers.ContainsKey(state))
            {
                return;
            }

            foreach (var (partyState, partyObject) in m_stateControllers)
            {
                partyObject.SetActive(partyState == state);
            }
        }
    }
}
