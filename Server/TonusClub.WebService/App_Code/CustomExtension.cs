using System;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace TonusClub.WebService
{
    public class CustomExtension : BehaviorExtensionElement, IServiceBehavior
    {
        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription,
            System.ServiceModel.ServiceHostBase serviceHostBase,
            System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints,
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {

        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {

        }

        #endregion

        public override Type BehaviorType
        {
            get { return typeof(CultureServiceBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new CultureServiceBehavior();
        }
    }
}