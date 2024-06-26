using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Gameplay;
using _Scripts.Schemas;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIMissionResults : MonoBehaviour, ISchemaController<SchemaMission>
    {
        [SerializeField] private TMP_Text m_rating;
        [SerializeField] private TMP_Text m_xpLabel;
        [SerializeField] private TMP_Text m_infamyLabel;
        [SerializeField] private Transform m_lootRoot;
        [SerializeField] private UILoot m_lootPrefab;
        
        [SerializeField] private Button m_accept;
        
        private SchemaMission m_mission;
        
        public void SetData(SchemaMission data)
        {
            m_mission = data;

            var missionResults = ServiceLocator.Instance.MissionManager.GetMissionInfo(data);
            
            m_rating.SetText((missionResults.m_score * 100).ToString("0"));
            m_xpLabel.SetText((missionResults.m_xp).ToString("0"));
            m_infamyLabel.SetText((missionResults.m_infamy).ToString("0"));

            // handle loot
            foreach (Transform child in m_lootRoot.transform)
            {
                Destroy(child.gameObject);
            }
            
            List<UILoot> loots = new List<UILoot>();

            for (var i = 0; i < missionResults.m_loot.Count; i++)
            {
                UILoot item = Instantiate(m_lootPrefab, m_lootRoot);
                item.SetData(missionResults.m_loot[i]);
                loots.Add(item);
            }

            StartCoroutine(DisplayRewards(loots));
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

        IEnumerator DisplayRewards(List<UILoot> loot)
        {
            foreach(var item in loot)
            {
                item.transform.localScale = Vector3.zero;
            }
            foreach (var item in loot)
            {
                item.transform.DOScale(1f, 1f).SetEase(Ease.OutBounce);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
