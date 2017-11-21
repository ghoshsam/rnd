using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Received.IntegrationEvents.Event
{
   public class CustomerCreateIntegrationEvent: IntegrationEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }

        public CustomerCreateIntegrationEvent(string firstName, string lastName, string emailId)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailId = emailId;
        }
    }
}
