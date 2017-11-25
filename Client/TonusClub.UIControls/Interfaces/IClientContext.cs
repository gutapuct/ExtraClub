using System;
using System.Threading.Tasks;
using TonusClub.ServiceModel;

namespace TonusClub.UIControls.Interfaces
{
    public interface IClientContext
    {
        bool CheckPermission(string permissionName);
        TResult ExecuteMethod<TResult>(Func<ITonusService, TResult> method);
        Task<TResult> ExecuteMethodAsync<TResult>(Func<ITonusService, TResult> method);
    }
}
