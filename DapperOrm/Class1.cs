using DataAccessWithDapper;
using System;

namespace DapperOrm
{
    public class ConnectionFactory : IDataFactory
    {
        public T CreateDatabase<T>()
        {
            throw new NotImplementedException();
        }
    }
}
