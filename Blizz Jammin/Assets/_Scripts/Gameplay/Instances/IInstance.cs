namespace _Scripts.Gameplay.Instances
{
    public abstract class Instance
    {
    }

    public interface IInstanceController<T> where T : Instance
    {
        void SetInstance(T instance);
    }
}