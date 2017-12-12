namespace DataAccessWithDapper
{
    public interface IDataFactory
    {
        T CreateDatabase<T>();

    }
}