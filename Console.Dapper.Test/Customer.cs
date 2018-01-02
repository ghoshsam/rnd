using DataAccessWithDapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Console.Dapper.Test
{
    public class Customer : Entity, IAggregateRoot
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
    }
}
