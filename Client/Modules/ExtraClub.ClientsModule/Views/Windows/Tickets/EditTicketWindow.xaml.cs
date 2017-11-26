using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Clients.Views.Windows.Tickets
{
    public partial class EditTicketWindow
    {
        public Ticket Ticket { get; set; }

        public int NewLength { get; set; }
        public int NewUnits { get; set; }
        public int NewGuest { get; set; }
        public int NewSolarium { get; set; }
        public int NewFreeze { get; set; }
        public string NewComment { get; set; }
        public string Comment { get; set; }
        public DateTime? NewInstalmentDate { get; set; }

        public EditTicketWindow(ClientContext context, Ticket ticket)
            : base(context)
        {
            Ticket = ticket;
            NewLength = ticket.Length;
            NewUnits = (int)ticket.UnitsAmount;
#if BEAUTINIKA
            NewGuest = (int)ticket.ExtraUnitsAmount;
#else
            NewGuest = (int)ticket.GuestUnitsAmount;
#endif
            NewSolarium = (int)ticket.SolariumMinutes;
            NewFreeze = ticket.FreezesAmount;

            NewInstalmentDate = ticket.PlanningInstalmentDay;
            NewComment = ticket.Comment;

            InitializeComponent();
            DataContext = this;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Comment))
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.EditReasonWarning);
                return;
            }
            if (Ticket.UnitsAmount - NewUnits > Ticket.UnitsLeft)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.UnitsWarning);
                return;
            }
#if BEAUTINIKA
            if (Ticket.ExtraUnitsAmount - NewGuest > Ticket.ExtraUnitsLeft)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                    "Невозможно списать дополнительных единиц больше, чем осталось!");
                return;
            }
#else
            if (Ticket.GuestUnitsAmount - NewGuest > Ticket.GuestUnitsLeft)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.GuestUnitsWarning);
                return;
            }
#endif
            if (Ticket.SolariumMinutes - NewSolarium > Ticket.SolariumMinutesLeft)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.SolMinutesWarning);
                return;
            }
            if (Ticket.FreezesAmount - NewFreeze > Ticket.FreezesLeft)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.FreezeDaysWarning);
                return;
            }
            if (NewLength < 0)
            {
                ExtraWindow.Alert(UIControls.Localization.Resources.Error,
                    UIControls.Localization.Resources.AbsLengthWarning);
                return;
            }
            _context.PostTicketCorrection(Ticket.Id, NewLength, NewUnits, NewGuest, NewSolarium, NewFreeze, Comment, NewInstalmentDate, NewComment);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
