using System;
using _Scripts.Schemas;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIPartyMonster : SerializedMonoBehaviour, ISchemaController<SchemaMonster>
    {
        [Serializable]
        public enum State
        {
            // Used in the party 
            Normal,
            Selected,
            Locked,
        }

        public Button Button => m_button;
        public SchemaMonster MonsterData => m_monsterData;
        
        [SerializeField] private UIMonsterDetails m_monsterDetails;
        [SerializeField] private Button m_button;
        
        [SerializeField] private GameObject m_noMonsterObject;
        [SerializeField] private GameObject m_hasMonsterObject;
        [SerializeField] private GameObject m_selectedObject;
        [SerializeField] private GameObject m_lockedObject;
        

        private SchemaMonster m_monsterData;

        public void SetState(State state)
        {
            switch (state)
            {
                case State.Normal:
                    m_selectedObject.SetActive(false);
                    m_lockedObject.SetActive(false);
                    return;
                case State.Selected:
                    m_selectedObject.SetActive(true);
                    m_lockedObject.SetActive(false);
                    return;
                case State.Locked:
                    m_selectedObject.SetActive(false);
                    m_lockedObject.SetActive(true);
                    return;
            }
        }
        
        public void SetData(SchemaMonster monster)
        {
            m_monsterData = monster;
            m_monsterDetails.SetData(monster);
            
            m_noMonsterObject.SetActive(m_monsterData == null);
            m_hasMonsterObject.SetActive(m_monsterData != null);
        }
    }
}
