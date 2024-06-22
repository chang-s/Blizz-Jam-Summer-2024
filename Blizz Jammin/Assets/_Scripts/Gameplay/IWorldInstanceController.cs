namespace _Scripts.Gameplay
{
    public interface IWorldInstanceController<T> where T : WorldInstance
    {
        // Find a way to type this better, so it does not incur a boxing in-and-out of the usage
        void SetInstance(T data);
    }
    
    public interface IController<T>
    {
        // Find a way to type this better, so it does not incur a boxing in-and-out of the usage
        void Set(T data);
    }
}