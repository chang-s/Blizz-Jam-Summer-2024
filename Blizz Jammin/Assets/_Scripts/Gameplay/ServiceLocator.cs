using _Scripts.Gameplay.Camera;
using _Scripts.UI;
using Utility;

namespace _Scripts.Gameplay
{
    /// <summary>
    /// This is the service locator pattern. All shared game systems will register to this singleton,
    /// and is the main way systems communicate.
    /// </summary>
    public class ServiceLocator : SingletonMonoBehaviour<ServiceLocator>
    {
        public CameraManager CameraManager { get; private set; }
        public MonsterManager MonsterManager { get; private set; }
        public UIPopupManager UIPopupManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            // Find all MonoBehaviour game systems.
            CameraManager = FindObjectOfType<CameraManager>();
            MonsterManager = FindObjectOfType<MonsterManager>();
            UIPopupManager = FindObjectOfType<UIPopupManager>();
        }
    }
}
