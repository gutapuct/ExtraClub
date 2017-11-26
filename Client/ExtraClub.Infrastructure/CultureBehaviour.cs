using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace ExtraClub.Infrastructure
{
    public class CultureBehaviour : IEndpointBehavior
    {
        public void ApplyClientBehavior(ServiceEndpoint endpoint,
              System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new CultureMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
              System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            //endpointDispatcher.DispatchRuntime.MessageInspectors.Add
            //                (new CultureMessageInspector());
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
}
