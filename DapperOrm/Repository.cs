using DataAccessWithDapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Data;
using System.Data.Common;

namespace Dapper.Data
{
    public class Repository<T> : IRepository<T> where T : Entity, IAggregateRoot
    {

        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _dbTransaction;

        public Repository() { }
        public Repository(IDbTransaction dbTransaction)
        {
           
            _dbConnection = dbTransaction.Connection;
           _dbTransaction = dbTransaction;

        }

        public T Add(T entity)
        {
            throw new NotImplementedException();
        }

        public T Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, int pageNo, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll(int pageNo, int pageSize)
        {
            throw new NotImplementedException();
        }

        public T FindById(Guid Id)
        {
            throw new NotImplementedException();
        }

        public T Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
