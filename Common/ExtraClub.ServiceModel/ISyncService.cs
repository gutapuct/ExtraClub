﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;


namespace ExtraClub.ServiceModel
{
    [ServiceContract]
    public interface ISyncService
    {
        [OperationContract]
        [FaultContract(typeof(string))]
        Stream GetInitialSyncData(string machineKey);

        [OperationContract]
        void PostSyncSuccess(string machineKey, long version);

        [OperationContract]
        [FaultContract(typeof(string))]
        Stream GetServerPart(string machineKey, int coherentVersion);

        [OperationContract]
        [FaultContract(typeof(string))]
        long PostClientPart(Stream part);

        [OperationContract]
        [FaultContract(typeof(int))]
        byte[] GetLicenceKey(string machineKey);

        [OperationContract]
        DateTime GetExpiryDate(string machineKey);

        [OperationContract]
        void PostApplicationErrorReport(string report);

        [OperationContract]
        [FaultContract(typeof(string))]
        void PostFrReport(string email, string subject, string repDetails, byte[] repFile);

        [OperationContract]
        [FaultContract(typeof(string))]
        Guid GetMainDivision(string machineKey);
    }
}