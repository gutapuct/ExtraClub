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

namespace ExtraClub.Clients.Views.Windows
{
    public partial class NewEditCrmEventWindow
    {
        public Customer Customer { get; set; }
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public string Result { get; set; }

        public NewEditCrmEventWindow(ClientContext context, Customer customer, CustomerEventView ev)
            : base(context)
        {
            InitializeComponent();
            Customer = customer;
            if (ev != null && ev.Id != Guid.Empty)
            {
                Id = ev.Id;
                Date = ev.Date;
                Subject = ev.TypeText;
                Comment = ev.Comments;
                Result = ev.Result;
            }
            else
            {
                Id = Guid.NewGuid();
                Date = DateTime.Now;
            }
            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostCrmEvent(Id, Customer.Id, Date, Subject, Comment, Result);
            DialogResult = true;
            Close();
        }
    }
}
