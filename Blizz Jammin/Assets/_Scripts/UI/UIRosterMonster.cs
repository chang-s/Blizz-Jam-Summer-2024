using System;
using _Scripts.Schemas;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIRosterMonster : SerializedMonoBehaviour, ISchemaController<SchemaMonster>
    {
        [Serializable]
        public enum State
        {
            Normal,
            Addable,
            InParty,
            InCombat,
            Selected,
        }

        public Button Button => m_button;
        public SchemaMonster MonsterData => m_monsterData;
        
        [SerializeField] private UIMonsterDetails m_monsterDetails;
        [SerializeField] private Button m_button;
        
        [SerializeField] private GameObject m_baseObject;
        [SerializeField] private GameObject m_addableObject;
        [SerializeField] private GameObject m_inPartyObject;
        [SerializeField] private GameObject m_inCombatObject;
        [SerializeField] private GameObject m_selectedObject;

        private SchemaMonster m_monsterData;

        public void SetState(State state)
        {
            m_baseObject.SetActive(true);
            
            switch (state)
            {
                case State.Normal:
                    m_addableObject.SetActive(false);
                    m_inPartyObject.SetActive(false);
                    m_inCombatObject.SetActive(false);
                    m_selectedObject.SetActive(false);
                    return;
                case State.Addable:
                    m_addableObject.SetActive(true);
                    m_inPartyObject.SetActive(false);
                    m_inCombatObject.SetActive(false);
                    m_selectedObject.SetActive(false);
                    return;
                case State.InParty:
                    m_addableObject.SetActive(false);
                    m_inPartyObject.SetActive(true);
                    m_inCombatObject.SetActive(false);
                    m_selectedObject.SetActive(false);
                    return;
                case State.InCombat:
                    m_addableObject.SetActive(false);
                    m_inPartyObject.SetActive(false);
                    m_inCombatObject.SetActive(true);
                    m_selectedObject.SetActive(false);
                    return;
                case State.Selected:
                    m_addableObject.SetActive(false);
                    m_inPartyObject.SetActive(false);
                    m_inCombatObject.SetActive(false);
                    m_selectedObject.SetActive(true);
                    return;
            }
        }
        
        public void SetData(SchemaMonster monster)
        {
            m_monsterData = monster;
            m_monsterDetails.SetData(monster);
        }
    }
}
