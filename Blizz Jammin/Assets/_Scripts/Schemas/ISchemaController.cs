namespace _Scripts.Schemas
{
    public interface ISchemaController<T> where T : Schema
    {
        // Find a way to type this better, so it does not incur a boxing in-and-out of the usage
        void SetData(T data);
    }
}