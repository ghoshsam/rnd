using DataAccessWithDapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Dapper.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConnectionFactory<DbConnection> _connectionFactory;
        private readonly IDbTransaction _dbTransaction;

        public UnitOfWork(IConnectionFactory<DbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _dbTransaction = connectionFactory.CreateDatabase().BeginTransaction();
        }

        public void Save()
        {
            _dbTransaction.Commit();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IRepository<T> CreateRepository<T>() where T: Entity, IAggregateRoot
        {            
            return new Repository<T>(_dbTransaction);
        }

        
    }
}
