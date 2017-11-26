using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ExtraClub.ServiceModel.Reports.ClauseParameters;
using System.Windows;

namespace ExtraClub.ServiceModel.Reports
{
    [DataContract]
    public class FiniteClause : Clause
    {
        private FiniteClause() { }

        [OnDeserialized]
        public void OnDeserialized()
        {
        }

        private Type _parameter;
        [XmlIgnore]
        public Type Parameter
        {
            get
            {
                if (_parameter == null && !String.IsNullOrEmpty(ParameterTypeName))
                {
                    _parameter = Type.GetType(ParameterTypeName);
                }
                return _parameter;
            }
            set
            {
                FixedValue = null;
                OnPropertyChanged("FixedValue");
               _parameter = value;
                ParameterTypeName = _parameter.FullName;
                UpdateAvailValues();
            }
        }
        public void UpdateAvailValues()
        {
            if (Parameter == null) return;
            if (AvailParameters[Parameter].GetValuesFunction != null && Context != null)
            {
                AvailValues = AvailParameters[Parameter].GetValuesFunction.Invoke(Context);
            }
            else
            {
                AvailValues = null;
            }
            OnPropertyChanged("AvailValues");
            OnPropertyChanged("Parameter");
            OnPropertyChanged("AvailOperators");
            OnPropertyChanged("DropdownValueVisibility");
            OnPropertyChanged("InputValueVisibility");
        }

        [XmlIgnore]
        public Visibility DropdownValueVisibility => AvailValues != null && AvailValues.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        [XmlIgnore]
        public Visibility InputValueVisibility => AvailValues == null || AvailValues.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        [XmlIgnore]
        public Dictionary<object, string> AvailValues { get; set; }

        private string _paramTypeName;
        [DataMember]
        public string ParameterTypeName
        {
            get
            {
                return _paramTypeName;
            }
            set
            {
                _paramTypeName = value;
                if (BaseType == null)
                {
                    if (Parameter != null)
                    {
                        BaseType = ((ClauseRelationAttribute)Parameter.GetCustomAttributes(typeof(ClauseRelationAttribute), false)[0]).RelatedEntityType;
                    }
                }
            }
        }

        private ClauseOperator _operator;
        [DataMember]
        public ClauseOperator Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
                OnPropertyChanged("Operator");
                OnPropertyChanged("ParametersVisibility");
                //UpdateAvailValues();
            }
        }

        [XmlIgnore]
        public Visibility ParametersVisibility => Operator != ClauseOperator.IsNotNull
                                                  && Operator != ClauseOperator.IsNull
                                                  && Operator != ClauseOperator.False
                                                  && Operator != ClauseOperator.True ? Visibility.Visible : Visibility.Collapsed;

        private object _fixedValue;
        [DataMember]
        public object FixedValue
        {
            get
            {
                return _fixedValue;
            }
            set
            {
                _fixedValue = value;
                OnPropertyChanged("FixedValue");
            }
        }

        private string _parameterName;
        [DataMember]
        public string ParameterName
        {
            get
            {
                return _parameterName;
            }
            set
            {
                _parameterName = value;
                OnPropertyChanged("ParameterName");
            }
        }

        private Dictionary<Type, ClauseParameter> _availParameters;

        [XmlIgnore]
        public Dictionary<Type, ClauseParameter> AvailParameters
        {
            get
            {
                if (_availParameters == null && BaseType != null)
                {
                    _availParameters = new Dictionary<Type, ClauseParameter>();
                    foreach (var i in ClauseRegistry.GetRelatedAttributes(BaseType))
                    {
                        var inst = (ClauseParameter)Activator.CreateInstance(i);
                        _availParameters.Add(i, inst);
                    }

                }
                return _availParameters;
            }
        }

        [XmlIgnore]
        public Dictionary<ClauseOperator, string> AvailOperators
        {
            get
            {
                if (Parameter == null) return new Dictionary<ClauseOperator, string>();
                return AvailParameters[Parameter].Operators.ToDictionary(i => i, Helper.GetText);
            }
        }

        bool _isFixedValue = true;
        [DataMember]
        public bool IsFixedValue
        {
            get
            {
                return _isFixedValue;
            }
            set
            {
                _isFixedValue = value;
                OnPropertyChanged("IsFixedValue");
                OnPropertyChanged("IsNotFixedValue");
            }
        }

        [XmlIgnore]
        public bool IsNotFixedValue
        {
            get
            {
                return !IsFixedValue;
            }
            set
            {
                IsFixedValue = !value;
            }
        }

        [XmlIgnore]
        public override bool IsFinite => true;

        [XmlIgnore]
        public Type BaseType { get; set; }

        public FiniteClause(Type baseType)
        {
            BaseType = baseType;
            _availParameters = new Dictionary<Type, ClauseParameter>();
            foreach (var i in ClauseRegistry.GetRelatedAttributes(baseType))
            {
                var inst = (ClauseParameter)Activator.CreateInstance(i);
                _availParameters.Add(i, inst);
            }
        }

        public void Convert(Clause clause)
        {
            var parent = Parent;
            clause.AttachLeft(this);
            clause.AttachRight(new FiniteClause(BaseType));
            if (parent == null) ReplaceMe(clause);
            else
            {
                if (parent.LeftPart == this)
                {
                    parent.AttachLeft(clause);
                }
                else if (parent.RightPart == this)
                {
                    parent.AttachRight(clause);
                }
            }
        }

        [XmlIgnore]
        public override string Name
        {
            get
            {
                if (Parameter == null || !AvailParameters.ContainsKey(Parameter)) return "#Ошибка#";
                return
                    $"{AvailParameters[Parameter].Name} {Operator} {((Operator == ClauseOperator.IsNotNull || Operator == ClauseOperator.IsNull) ? (String.Empty) : (_isFixedValue ? String.Format("\"{0}\"", FixedValue) : String.Format("[{0}]", ParameterName)))}";
            }
        }
    }

}
