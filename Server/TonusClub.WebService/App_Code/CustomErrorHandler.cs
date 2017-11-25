using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;

public class CustomErrorHandler : IErrorHandler
{
    public bool HandleError(Exception error)
    {
        TonusClub.ServerCore.Logger.Log(error);
        return true;
    }

    public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
    {
        fault = Message.CreateMessage(
            version,
            new FaultException<string>(error.Message, new FaultReason(error.Message)).CreateMessageFault(),
            "http://the.fault.action");
    }

}

[AttributeUsage(AttributeTargets.Class)]
public class ErrorHandlerBehaviorAttribute : Attribute, IServiceBehavior
{
    /// <summary>
    /// Обработчик ошибок
    /// </summary>
    private IErrorHandler _errorHandler;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="typeErrorHandler">Тип обработчика ошибок</param>
    public ErrorHandlerBehaviorAttribute(Type typeErrorHandler)
    {
        if (typeErrorHandler == null) throw new ArgumentNullException();

        _errorHandler = (IErrorHandler)Activator.CreateInstance(typeErrorHandler);
    }

    #region IServiceBehavior Members

    void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription,
        ServiceHostBase serviceHostBase)
    {
        // Все диспетчеры хоста связываем с ранее указанным обработчиком ошибок
        foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
        {
            dispatcher.ErrorHandlers.Add(_errorHandler);
        }
    }

    void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription,
        ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
        BindingParameterCollection bindingParameters)
    {
    }

    void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
    {
    }

    #endregion
}
