using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExtraClub.ServiceModel
{
    partial class Anket : IDataErrorInfo, IInitable
    {
        private static Dictionary<string, string> PropertiesBindings = new Dictionary<string, string>();

        static Anket()
        {
            PropertiesBindings.Add("EmployeesGrade", "EmployeesGradeDesc");
            PropertiesBindings.Add("ClubDevGrade", "ClubDevDesc");
            PropertiesBindings.Add("SelfGrade", "SelfDesc");
            PropertiesBindings.Add("FranchGrade", "FranchDesc");
            PropertiesBindings.Add("FranchSuppGrade", "FranchSuppDesc");
            PropertiesBindings.Add("AsuSuppGrade", "AsuSuppDesc");
            PropertiesBindings.Add("SiteAdmGrade", "SiteAdmDesc");
            PropertiesBindings.Add("DesignerGrade", "DesignerDesc");
            PropertiesBindings.Add("AccountantsGrade", "AccountantsDesc");
            PropertiesBindings.Add("LogistGrade", "LogistDesc");
            PropertiesBindings.Add("RepairGrade", "RepairDesc");
            PropertiesBindings.Add("BeautyNatureGrade", "BeautyNatureDesc");
            PropertiesBindings.Add("BeInformedGrade", "BeInformedDesc");
            PropertiesBindings.Add("HadSelfActions", "SelfActions");
            PropertiesBindings.Add("Meeting", "MeetingDesc");
            PropertiesBindings.Add("Test", "TestDesc");
            PropertiesBindings.Add("NewTreatments", "NewTreatmentsDesc");
            PropertiesBindings.Add("TreatmentProblems", "TreatmentProblemsDesc");
            PropertiesBindings.Add("ClubInfo", "ClubInfoDesc");
            PropertiesBindings.Add("ClubNews", "ClubNewsDesc");
        }

        partial void OnPropertyChangedInternal(string propertyName)
        {
            if (PropertiesBindings.ContainsKey(propertyName))
            {
                OnPropertyChanged(PropertiesBindings[propertyName]);
            }
        }

        [DataMember]
        public List<AnketTicket> SerializedAnketTickets { get; set; }
        [DataMember]
        public List<AnketTreatment> SerializedAnketTreatments { get; set; }
        [DataMember]
        public List<AnketAdvert> SerializedAnketAdverts{ get; set; }

        public string StatusDescription
        {
            get
            {
                if (StatusId == 0) return "Черновик";
                if (StatusId == 1) return "Отправлена";
                if (StatusId == 2) return "Получена франчайзором";
                return "Некорректный статус";
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!String.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch(columnName){
                    case "NetActions":
                        if (String.IsNullOrWhiteSpace(NetActions)) return "!";
                        break;
                    case "SelfActions":
                        if (String.IsNullOrWhiteSpace(SelfActions) && HadSelfActions) return "!";
                        break;
                    case "NextNetActions":
                        if (String.IsNullOrWhiteSpace(NextNetActions)) return "!";
                        break;
                    case "EmployeesGrade":
                    case "ClubDevGrade":
                    case "SelfGrade":
                    //case "FranchGrade":
                    //case "FranchSuppGrade":
                    //case "AsuSuppGrade":
                    //case "DesignerGrade":
                    //case "SiteAdmGrade":
                    //case "AccountantsGrade":
                    //case "LogistGrade":
                    //case "RepairGrade":
                    //case "BeautyNatureGrade":
                    //case "BeInformedGrade":
                        if (GetValue(this, columnName) == null) return "!";
                        break;
                    case "EmployeesGradeDesc":
                        if (String.IsNullOrWhiteSpace(EmployeesGradeDesc) && EmployeesGrade != 0) return "!";
                        break;
                    case "MeetingDesc":
                        if (String.IsNullOrWhiteSpace(MeetingDesc) && Meeting) return "!";
                        break;
                    case "TestDesc":
                        if (String.IsNullOrWhiteSpace(TestDesc) && Test) return "!";
                        break;
                    case "NewTreatmentsDesc":
                        if (String.IsNullOrWhiteSpace(NewTreatmentsDesc) && NewTreatments) return "!";
                        break;
                    case "TreatmentProblemsDesc":
                        if (String.IsNullOrWhiteSpace(TreatmentProblemsDesc) && TreatmentProblems) return "!";
                        break;
                    case "ClubInfoDesc":
                        if (String.IsNullOrWhiteSpace(ClubInfoDesc) && ClubInfo) return "!";
                        break;
                    case "ClubNewsDesc":
                        if (String.IsNullOrWhiteSpace(ClubNewsDesc) && ClubNews) return "!";
                        break;
                    case "ClubDevDesc":
                        if(String.IsNullOrWhiteSpace(ClubDevDesc) && ClubDevGrade > 0) return "!";
                        break;
                    case "SelfDesc":
                        if(String.IsNullOrWhiteSpace(SelfDesc) && SelfGrade > 0) return "!";
                        break;
                    case "FranchDesc":
                        if (String.IsNullOrWhiteSpace(FranchDesc) && FranchGrade > 0) return "!";
                        break;
                    case "FranchSuppDesc":
                        if (String.IsNullOrWhiteSpace(FranchSuppDesc) && FranchSuppGrade > 0) return "!";
                        break;
                    case "AsuSuppDesc":
                        if (String.IsNullOrWhiteSpace(AsuSuppDesc) && AsuSuppGrade > 0) return "!";
                        break;
                    case "DesignerDesc":
                        if (String.IsNullOrWhiteSpace(DesignerDesc) && DesignerGrade > 0) return "!";
                        break;
                    case "SiteAdmDesc":
                        if (String.IsNullOrWhiteSpace(SiteAdmDesc) && SiteAdmGrade > 0) return "!";
                        break;
                    case "AccountantsDesc":
                        if (String.IsNullOrWhiteSpace(AccountantsDesc) && AccountantsGrade > 0) return "!";
                        break;
                    case "LogistDesc":
                        if (String.IsNullOrWhiteSpace(LogistDesc) && LogistGrade > 0) return "!";
                        break;
                    case "RepairDesc":
                        if (String.IsNullOrWhiteSpace(RepairDesc) && RepairGrade > 0) return "!";
                        break;
                    case "BeautyNatureDesc":
                        if (String.IsNullOrWhiteSpace(BeautyNatureDesc) && BeautyNatureGrade > 0) return "!";
                        break;
                    //case "BeInformedDesc":
                    //    if (String.IsNullOrWhiteSpace(BeInformedDesc) && BeInformedGrade > 0) return "!";
                    //    break;
                    case "IncomeFactors":
                        if (String.IsNullOrWhiteSpace(IncomeFactors)) return "!";
                        break;
                    //case "Wishes":
                    //    if (String.IsNullOrWhiteSpace(Wishes)) return "!";
                    //    break;
                    case "FilledBy":
                        if (String.IsNullOrWhiteSpace(FilledBy)) return "!";
                        break;
                    case "FilledByPosition":
                        if (String.IsNullOrWhiteSpace(FilledByPosition)) return "!";
                        break;
                }
                return null;
            }
        }

        public static object GetValue(object obj, string propertyName)
        {
            if (obj == null || String.IsNullOrEmpty(propertyName)) return null;

            var attrs = propertyName.Split('.');

            if (attrs.Count() > 1)
            {
                foreach (var attr in attrs)
                {
                    obj = GetValue(obj, attr);
                    if (obj == null) return null;
                }
                return obj;
            }

            var pi = obj.GetType().GetProperty(propertyName);
            if (pi == null) return null;
            return pi.GetValue(obj, null);
        }

        public void Init()
        {
            SerializedAnketTickets = AnketTickets.ToList();
            SerializedAnketTickets.ForEach(i=>i.SerializedName = i.TicketType.Name);
            SerializedAnketAdverts = AnketAdverts.ToList();
            SerializedAnketTreatments = AnketTreatments.ToList();
            SerializedAnketTreatments.ForEach(i => i.SerializedName = i.TreatmentType.Name);
        }
    }
}
