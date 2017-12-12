using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;


namespace DataAccessWithDapper
{
    public class Repository<T> : IRepository<T> where T: Entity,IAggregateRoot 
    {

        private readonly IDbConnection _dbConnection;
        public Repository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;

        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, int pageNo, int pageSize)
        {
            
        }

        public IEnumerable<T> FindAll(int pageNo, int pageSize)
        {
            throw new NotImplementedException();
        }

        public T FindById(Guid Id)
        {
            throw new NotImplementedException();
        }

        public T Add(T entity)
        {
            throw new NotImplementedException();
        }
        public T Update(T entity)
        {
            throw new NotImplementedException();
        }
        public T Delete(T entity)
        {
            throw new NotImplementedException();
        }

    }
}
