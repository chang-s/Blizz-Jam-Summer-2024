using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Gameplay.Camera
{
    public class CameraManager : SerializedMonoBehaviour
    {
        [Serializable]
        public enum CameraLocation
        {
            Lair,
            Missions
        }
        
        [SerializeField] private UnityEngine.Camera m_camera;
        [SerializeField] private Dictionary<CameraLocation, Transform> m_configs;

        public void SetCameraLocation(CameraLocation location)
        {
            if (!m_configs.ContainsKey(location))
            {
                return;
            }

            m_camera.transform.DOMove(
                m_configs[location].position,
                0.25f,
                true
            );
        }
    }
}
