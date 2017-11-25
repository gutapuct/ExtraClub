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
using TonusClub.UIControls.Windows;
using Telerik.Windows.Controls;
using TonusClub.OrganizerModule.Business;
using TonusClub.UIControls;

namespace TonusClub.OrganizerModule.Views.Claims.Windows
{
    public partial class ClaimDetails
    {
        public Claim Claim { get; set; }
        public string ClaimTypeText { get; set; }
        public string Equipment { get; set; }

        public Visibility EqVisible { get; set; }
        public Visibility ResVisible { get; set; }
        public Visibility TiVisible { get; set; }
        public Visibility CommitExecVisible { get; set; }

        public object Comments { get; set; }


        public ClaimDetails(ClientContext context, Claim claim)
            : base(context)
        {
            Claim = claim;
            ClaimTypeText = ClaimTypesClass.ClaimTypes[claim.ClaimTypeId];

            EqVisible = claim.ClaimTypeId == 0 ? Visibility.Visible : Visibility.Collapsed;
            TiVisible = (claim.ClaimTypeId == 6 || claim.ClaimTypeId == 7) ? Visibility.Visible : Visibility.Collapsed;
            ResVisible = claim.FinishDate.HasValue ? Visibility.Visible : Visibility.Collapsed;
            CommitExecVisible = claim.StatusId == 4 || claim.StatusDescription == "Исполнена - подтвердите" ? Visibility.Visible : Visibility.Hidden;
            if(claim.Eq_TreatmentId.HasValue)
            {
                Equipment = context.GetAllTreatments().Where(i => i.Id == claim.Eq_TreatmentId).Select(i => i.DisplayName).FirstOrDefault();
            }
            DataContext = this;
            if(claim.FtmId.HasValue)
            {
                RefreshComments();
            }
            InitializeComponent();
            AddCommentButton.Visibility = claim.FtmId.HasValue ? Visibility.Visible : Visibility.Collapsed;

#if BEAUTINIKA
            addrText.Text = "Страна. Точный адрес студии (с индексом)";
#endif
        }

        private void RefreshComments()
        {
            FtmUtility.GetCommentsAsync(Claim.FtmId.Value, res =>
            {
                Comments = res;
                OnPropertyChanged("Comments");
            });
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Prompt(
                "Оценка запроса",
                "Пожалуйста, оцените выполнение запроса от 0 до 10:",
                "10",
                w => ScoreClaim(w)); 
        }

        private void ScoreClaim(PromptWindow e)
        {
            if (e.DialogResult ?? false)
            {
                int score;
                if (!Int32.TryParse(e.TextResult, out score))
                {
                    TonusWindow.Alert("Ошибка", "Необходимо оценить выполнение запроса!");
                    return;
                }

                if (score < 0 || score > 10)
                {
                    TonusWindow.Alert("Ошибка", "Пожалуйста, оцените выполнение запроса в пределах от 0 до 10!");
                    return;
                }
                _context.SubmitClaim(Claim.Id, score);
                DialogResult = true;
                Close();
            }
        }

        private void Reopen_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Prompt(
                 "Дополнительная информация",
                 "Пожалуйста, укажите причину возобновления задачи:",
                 "",
                w => EditClosed(w)
            );
        }

        private void EditClosed(PromptWindow e)
        {
            if(e.DialogResult ?? false)
            {
                if(String.IsNullOrWhiteSpace(e.TextResult))
                {
                    TonusWindow.Alert("Ошибка", "Необходимо указать причину возобновления задачи!");
                    return;
                }
                _context.ReopenClaim(Claim.Id, e.TextResult.Trim());
                DialogResult = true;
                Close();
            }
        }

        private void Comment_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Prompt("Добавление комментария", "Введите текст коментария:", "", w =>
            {
                if((w.DialogResult ?? false) && !String.IsNullOrWhiteSpace(w.TextResult))
                {
                    FtmUtility.PostComment(Claim.FtmId.Value, w.TextResult.Trim());
                    RefreshComments();
                }
            });
        }

        private void Complaint_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Prompt("Оставить жалобу", "Введите текст жалобы:", "", w =>
            {
                if ((w.DialogResult ?? false) && !String.IsNullOrWhiteSpace(w.TextResult))
                {
                    FtmUtility.PostComment(Claim.FtmId.Value, "Жалоба: " + w.TextResult.Trim());
                    FtmUtility.PostComplaint(Claim.FtmId.Value, w.TextResult.Trim());
                    RefreshComments();
                }
            });
        }
    }
}
