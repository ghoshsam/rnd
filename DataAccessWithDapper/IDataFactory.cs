namespace DataAccessWithDapper
{
    public interface IConnectionFactory<T>
    {
        T CreateDatabase();

    }
}