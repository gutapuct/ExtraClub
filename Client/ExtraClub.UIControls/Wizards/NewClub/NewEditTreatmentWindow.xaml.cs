using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.ClientDal.Wizards.NewClub
{
    public partial class NewEditTreatmentWindow
    {
        private IDictionaryManager _dictMan;

        public ICollectionView TreatmentTypesView{ get; set; }

        private IEnumerable<TreatmentType> _treatmentTypes;

        public Treatment Treatment { get; set; }

        ClientContext _context;

        public NewEditTreatmentWindow(IDictionaryManager dictMan, ClientContext context, Treatment treatment, Guid divId)
        {
            DataContext = this;
            _context = context;
            _dictMan = dictMan;

            _treatmentTypes = context.GetAllTreatmentTypes().Where(i=>i.IsActive).ToList();

            if (treatment == null || treatment.Id == Guid.Empty)
            {
                Treatment = new Treatment
                {
                    AuthorId = context.CurrentUser.UserId,
                    CreatedOn = DateTime.Now,
                    DivisionId = divId,
                    IsActive = true,
                    MaxCustomers = 1
                };
            }
            else
            {
                Treatment = treatment;
            }
            TreatmentTypesView = CollectionViewSource.GetDefaultView(_treatmentTypes);
            Treatment.PropertyChanged += Treatment_PropertyChanged;
            InitializeComponent();
            if (Treatment.Id != Guid.Empty)
            {
                type.IsEditable = false;
            }
            FixEnabled();
        }

        void Treatment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TreatmentTypeId" && Treatment.TreatmentTypeId != Guid.Empty)
            {
                FixEnabled();
            }
        }

        private void FixEnabled()
        {
            var tt = _treatmentTypes.FirstOrDefault(t => t.Id == Treatment.TreatmentTypeId);
            if (tt == null)
            {
                maxCustEdit.IsEnabled = false;
                Treatment.MaxCustomers = 1;
                return;
            }
            if (!(maxCustEdit.IsEnabled = tt.AllowsMultiple))
            {
                Treatment.MaxCustomers = 1;
            }
            durationText.Text = tt.Duration + " мин.";
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Treatment.TreatmentTypeId == Guid.Empty) return;
            _context.PostTreatment(Treatment);
            DialogResult = true;
            Close();
        }
    }
}
