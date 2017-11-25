using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace TonusClub.ServiceModel.Reports
{
    public abstract class Clause : INotifyPropertyChanged
    {
        [XmlIgnore]
        public dynamic Context { get; private set; }

        [XmlIgnore]
        public abstract string Name { get; }

        [XmlIgnore]
        public abstract bool IsFinite { get; }

        private Clause _leftPart;

        [DataMember]
        public Clause LeftPart
        {
            get
            {
                return _leftPart;
            }
            set
            {
                _leftPart = value;
                OnPropertyChanged("LeftPart");
            }
        }

        private Clause _rightPart;

        [DataMember]
        public Clause RightPart
        {
            get
            {
                return _rightPart;
            }
            set
            {
                _rightPart = value;
                OnPropertyChanged("RightPart");
            }
        }

        [XmlIgnore]
        public Clause Parent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ClauseArgs> Replace;

        public Clause AttachLeft(Clause clause)
        {
            clause.Parent = this;
            if (IsFinite) throw new Exception();
            clause.SetContext(Context);
            return LeftPart = clause;
        }

        public Clause AttachRight(Clause clause)
        {
            clause.Parent = this;
            if (IsFinite) throw new Exception();
            clause.SetContext(Context);
            return RightPart = clause;
        }

        public void RemoveMe(bool removeLeft)
        {
            var param = !removeLeft ? LeftPart : RightPart;
            if (param == null) throw new ArgumentNullException(nameof(removeLeft));

            if (Parent == null)
            {
                param.Parent = null;
                ReplaceMe(param/* ?? new FiniteClause()*/);
            }
            else
            {
                if (Parent.LeftPart == this)
                {
                    Parent.AttachLeft(param/* ?? new FiniteClause(BaseType)*/);
                }
                else if (Parent.RightPart == this)
                {
                    Parent.AttachRight(param/* ?? new FiniteClause(BaseType)*/);
                }
            }
            OnPropertyChanged("SomeChildProperty");
        }

        protected void ReplaceMe(Clause clause)
        {
            Replace?.Invoke(this, new ClauseArgs { NewClause = clause });
            OnPropertyChanged("SomeChildProperty");
        }

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            Parent?.OnPropertyChanged("SomeChildProperty");
        }

        public override string ToString()
        {
            if (IsFinite) return Name;
            return $"({LeftPart}) {Name} ({RightPart})";
        }

        public void SetContext(dynamic context)
        {
            Context = context;
            LeftPart?.SetContext(context);
            RightPart?.SetContext(context);
            (this as FiniteClause)?.UpdateAvailValues();
        }
    }

    public class ClauseArgs : EventArgs
    {
        public Clause NewClause { get; set; }
    }
}
