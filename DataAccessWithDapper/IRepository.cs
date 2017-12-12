using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccessWithDapper
{
    public interface IRepository<T> where T: Entity, IAggregateRoot
    {

        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
        T FindById(Guid Id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate, int pageNo, int pageSize);
        IEnumerable<T> FindAll(int pageNo,int pageSize);
              
    }
}
