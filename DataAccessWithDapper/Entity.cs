using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessWithDapper
{
    public class Entity
    {
        //IEnumerable<ExtendedProperty> _extendedProperty = null;
        int TenantId { get; set; }
        IEnumerable<ExtendedProperty> ExtendedProperties { get; set; }
    }
}
