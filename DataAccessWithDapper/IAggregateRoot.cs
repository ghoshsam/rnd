using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessWithDapper
{
    public interface IAggregateRoot
    {
        Guid EntityId { get; set; }
    }
}
