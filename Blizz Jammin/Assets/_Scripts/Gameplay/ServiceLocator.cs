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
        public UIPopupManager UIPopupManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            // Find all MonoBehaviour game systems.
            UIPopupManager = FindObjectOfType<UIPopupManager>();
        }
    }
}
