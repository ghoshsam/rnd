using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessWithDapper
{
    interface IUnitOfWork:IDisposable
    {
        void Save();
    }
}
