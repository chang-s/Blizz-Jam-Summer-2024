using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay.Camera
{
    public class CameraMoveButton : MonoBehaviour
    {
        [SerializeField] private CameraManager.CameraLocation m_location;
        [SerializeField] private Button m_button;

        private void Awake()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            ServiceLocator.Instance.CameraManager.SetCameraLocation(m_location);
        }
    }
}
