using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ClaimServiceContract
{
    [ServiceContract]
    public interface IClaimService
    {
        [OperationContract]
        int AddClaim(Guid claimId, int typeId, Guid companyId, string companyName, string contactEmail, string contactName,
            string phone, DateTime createdOn, string eq_BuyDate, string eq_Guaranty, string eq_Serial,
            string message, string prefFinishDate, string subject, string circulation);

        [OperationContract]
        int AddClaimEx(Guid claimId, int typeId, Guid companyId, string companyName, string contactEmail, string contactName,
            string phone, DateTime createdOn, string eq_BuyDate, string eq_Guaranty, string eq_Serial,
            string message, string prefFinishDate, string subject, string circulation, Guid? eq_TreatmentId, string eq_TechContact,
            string eq_SerialGutwell, string eq_Model, string eq_ClubAddr, string eq_PostAddr);

        [OperationContract]
        Guid CreateTask(TaskInfo info);

        [OperationContract]
        ClaimInfo GetClaimInfo(int claimNumber);

        [OperationContract]
        void SubmitClaim(int claimNumber);

        [OperationContract]
        bool CheckClaimClosed(int claimNumber);

        [OperationContract]
        void ReopenClaim(int claimNumber, string reopenMessage);

        [OperationContract]
        Task AddCustomerCommentAsync(string asuDivisionId, string message);
    }
}
