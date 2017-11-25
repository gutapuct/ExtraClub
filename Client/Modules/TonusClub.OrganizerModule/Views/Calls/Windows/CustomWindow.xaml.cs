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
using System.Windows.Shapes;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Calls.Windows
{
    public partial class CustomWindow
    {
        public IncomingCallForm Form { get; set; }
        public IncomingCallFormButton ButtonResult { get; set; }
        public List<File> Links { get; set; }
        public string TextResult { get; set; }

        StringBuilder Log;
        ClientContext _context;

        public CustomWindow(IncomingCallForm form, List<File> links, ClientContext context, StringBuilder log)
        {
            _context = context;
            Form = form;
            Links = links;
            Log = log;
            InitializeComponent();
            DataContext = this;
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var desc = ((FrameworkElement)sender).DataContext as IncomingCallFormButton;
            if (desc.ButtonAction == 1 || desc.ButtonAction==0)
            {
                ButtonResult = desc;
                Close();
            }
            if (desc.ButtonAction == 2)
            {
                Log.AppendLine("Активирована сетка процедур");
                NavigationManager.MakeActivateTreatmentsScheduleRequest(); 
                if(!desc.IsFinal) return;
                ButtonResult = desc;
                Close();
            }
            if (desc.ButtonAction == 3)
            {
                Log.AppendLine("Активирована сетка солярия");
                NavigationManager.MakeActivateSolariumScheduleRequest();
                if (!desc.IsFinal) return;
                ButtonResult = desc;
                Close();
            }
            if (desc.ButtonAction == 4)
            {
                Log.AppendLine("Активирована запись в солярий");
                NavigationManager.MakeNewSolariumVisitRequest(new Infrastructure.ParameterClasses.NewSolariumVisitParams
                {
                    CloseAction = () =>
                    {
                        if (!desc.IsFinal) return;
                        ButtonResult = desc;
                        Close();
                    }
                });
            }
            if (desc.ButtonAction == 5)
            {
                Log.AppendLine("Активировано создание нового клиента");
                NavigationManager.MakeNewClientRequest(clientId =>
                {
                    Log.AppendFormat("ID нового клиента: {0}\n", clientId);
                    if (!desc.IsFinal) return;
                    ButtonResult = desc;
                    Close();
                });
            }
            if (desc.ButtonAction == 6)
            {
                Log.AppendLine("Активировано создание нового клиента и запись его на услуги");
                NavigationManager.MakeNewClientRequest(clientId =>
                {
                    Log.AppendFormat("ID нового клиента: {0}\n", clientId);
                    if (clientId != Guid.Empty)
                    {
                        NavigationManager.MakeNewSolariumVisitRequest(new Infrastructure.ParameterClasses.NewSolariumVisitParams { CustomerId = clientId });
                    }
                    if (!desc.IsFinal) return;
                    ButtonResult = desc;
                    Close();
                });
            }
        }

        private void LinkClicked(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is File)
            {
                var res = _context.DownloadFile(((FrameworkElement)sender).DataContext as File);
                System.Diagnostics.Process.Start(res);
            }
        }
    }
}
