using System;
using System.Collections.Generic;
using System.Windows;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.TreatmentsModule.Views.Windows
{
    /// <summary>
    /// Interaction logic for BrokenTreatmentWindow.xaml
    /// </summary>
    public partial class BrokenTreatmentWindow
    {
        public Guid SelectedTreatmentId { get; set; }
        public List<Treatment> TreatmentsView { get; set; }

        public BrokenTreatmentWindow(ClientContext context)
            : base(context)
        {
            InitializeComponent();
            DataContext = this;
            TreatmentsView = _context.GetAllTreatments();
        }

        private void RadButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void CommitButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTreatmentId == Guid.Empty) return;
            _context.PostTreatmentBreakdown(SelectedTreatmentId);
            DialogResult = true;
            Close();
        }
    }
}
