using EventBus.Abstractions;
using Received.IntegrationEvents.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Received.IntegrationEvents.Handler
{
    public class CustomerCreateIntegrationEventHandler : IIntegrationEventHandler<CustomerCreateIntegrationEvent>
    {
        string test = "";
        public CustomerCreateIntegrationEventHandler()
        {
            test = "Test";
        }
        public async Task Handle(CustomerCreateIntegrationEvent @event)
        {
            Console.WriteLine("First Name :{0}", @event.FirstName);
            //await new Task(() => {
            //     Console.WriteLine("First Name :{0}", @event.FirstName);
            //     Console.WriteLine("Last Name :{0}", @event.LastName);
            //     Console.WriteLine("Email Id :{0}", @event.EmailId);
            // });


        }
    }
}
