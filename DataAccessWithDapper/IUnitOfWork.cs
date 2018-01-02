using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessWithDapper
{
    public interface IUnitOfWork:IDisposable
    {
        void Save();
        IRepository<T> CreateRepository<T>() where T: Entity,IAggregateRoot;

    }
}
