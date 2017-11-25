using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel
{
    partial class TreatmentProgram : IInitable, IDataErrorInfo
    {
        [DataMember]
        public List<TreatmentProgramLine> SerializedTreatmentProgramLines { get; set; }

        [DataMember]
        public string SerializedNextProgramName { get; private set; }
        [DataMember]
        public string SerializedContent { get; private set; }

        public void Init()
        {
            SerializedTreatmentProgramLines = TreatmentProgramLines.ToList();
            SerializedTreatmentProgramLines.Sort();

            SerializedNextProgramName = NextProgramId.HasValue ? NextProgram.ProgramName : String.Empty;

            SerializedContent = String.Empty;
            foreach(var i in TreatmentProgramLines.OrderBy(i=>i.Position)) {
                if (!String.IsNullOrEmpty(SerializedContent)) SerializedContent += "; ";
                SerializedContent += i.TreatmentConfig.Name;
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
                switch (columnName)
                {
                    case "ProgramName":
                        if (String.IsNullOrWhiteSpace(ProgramName)) return "!";
                        break;
                }
                return String.Empty;
            }
        }
    }
}
