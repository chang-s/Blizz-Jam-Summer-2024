using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay
{
    public class NavigationManager : SerializedMonoBehaviour
    {
        [Serializable]
        private struct NavigationConfig
        {
            public Transform m_cameraDolly;
            public Button m_button;
        }
        
        [Serializable]
        public enum NavigationItemType
        {
            Lair,
            Missions
        }
        
        [SerializeField] private Camera m_camera;
        [SerializeField] private Dictionary<NavigationItemType, NavigationConfig> m_configs;

        private void Awake()
        {
            foreach (var (typeIter, configIter) in m_configs)
            {
                configIter.m_button.onClick.AddListener(() =>
                {
                    SetCameraLocation(typeIter);
                });
            }
        }

        private void SetCameraLocation(NavigationItemType type)
        {
            if (!m_configs.ContainsKey(type))
            {
                return;
            }

            foreach (var (typeIter, configIter) in m_configs)
            {
                if (typeIter == type)
                {
                    // Move to the given position
                    m_camera.transform.DOMove(
                        configIter.m_cameraDolly.position,
                        0.25f
                    );
                    
                    // Hide the button used to get here
                    configIter.m_button.gameObject.SetActive(false);
                }
                else
                {
                    // Turn on all other nav items
                    configIter.m_button.gameObject.SetActive(true);
                }
            }

        }
    }
}
