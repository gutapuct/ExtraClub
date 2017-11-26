using System;
using System.Threading.Tasks;
using ExtraClub.ServiceModel;

namespace ExtraClub.UIControls.Interfaces
{
    public interface IClientContext
    {
        bool CheckPermission(string permissionName);
        TResult ExecuteMethod<TResult>(Func<IExtraService, TResult> method);
        Task<TResult> ExecuteMethodAsync<TResult>(Func<IExtraService, TResult> method);
    }
}
