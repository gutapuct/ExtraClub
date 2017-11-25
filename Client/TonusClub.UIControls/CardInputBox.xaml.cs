using System;
using System.Windows;
using System.ComponentModel;
using TonusClub.UIControls.Windows;

namespace TonusClub.UIControls
{
    public partial class CardInputBox : INotifyPropertyChanged
    {
        public CardInputBox()
        {
            InitializeComponent();
            parsecReader = new ParsecReader(0);
            parsecReader.CardChanged += new EventHandler<CardEventArgs>(parsecReader_CardChanged);
        }

        public string SelectedCard
        {
            get { return base.GetValue(SelectedCardProperty) as string; }
            set
            {
                base.SetValue(SelectedCardProperty, value);
                OnPropertyChanged("ToggleCaption");
            }
        }
        public static readonly DependencyProperty SelectedCardProperty =
            DependencyProperty.Register("SelectedCard", typeof(string), typeof(CardInputBox), null);


        public string ToggleCaption
        {
            get
            {
                if (parsecReader.IsListening) return "Поднесите карту...";
                return SelectedCard;
            }
        }



        void parsecReader_CardChanged(object sender, CardEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SelectedCard = e.CardNumber.ToString();
                IsListening = false;
            }));
        }

        private bool _isListening = false;
        public bool IsListening
        {
            get
            {
                return _isListening;
            }
            set
            {
                _isListening = value;

                if (_isListening)
                {
                    try
                    {
                        parsecReader.StartListening();
                    }
                    catch (Exception)
                    {
                        TonusWindow.Prompt(UIControls.Localization.Resources.ManualInput,
                            UIControls.Localization.Resources.ProvideCardNumber,
                            "",
                            (wnd) => EditClosed(wnd));
                    }
                }
                else
                {
                    parsecReader.StopListening();
                }
                OnPropertyChanged("ToggleCaption");
                OnPropertyChanged("IsListening");
            }
        }

        private void EditClosed(PromptWindow wnd)
        {
            if (wnd.DialogResult ?? false)
            {
                var s = (wnd.TextResult ?? "").Trim();
                int i;
                if (Int32.TryParse(s, out i))
                {
                    parsecReader_CardChanged(null, new CardEventArgs { CardNumber = i });
                }
            }
            IsListening = false;
        }

        ParsecReader parsecReader;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ManualInput(object sender, RoutedEventArgs e)
        {
            TonusWindow.Prompt(UIControls.Localization.Resources.ManualInput,
                UIControls.Localization.Resources.ProvideCardNumber,
                "",
                (wnd) => EditClosed(wnd));
        }
    }
}
