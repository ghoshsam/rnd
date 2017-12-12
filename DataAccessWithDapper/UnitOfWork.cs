using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessWithDapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _dbConnection;
        private readonly DbProviderFactory _dbProvider;
        private readonly IDbTransaction _dbTransaction;
        private readonly IServiceProvider serviceProvider;
        public UnitOfWork(IDataFactory dataFactory)
        {
            _dbConnection = dataFactory.CreateDatabase<IDbConnection>();
            _dbTransaction = _dbConnection.BeginTransaction();
            //IDbConnection 
        }

        public IRepository<T> Repository<T>() where T: Entity,IAggregateRoot
        {
            return serviceProvider.GetService<IRepository<T>>();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _dbTransaction.Commit();
        }
    }
}
