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
using System.IO;
using System.ServiceModel;

namespace ExtraClub.Clients.Views
{
    /// <summary>
    /// Interaction logic for UploadImage.xaml
    /// </summary>
    public partial class UploadImage
    {
        public Customer Customer { get; set; }

        string _newPathText;
        public string NewPathText { get { return _newPathText; }
            set
            {
                _newPathText = value;
                OnPropertyChanged("NewPathText");
            }
        }

        public byte[] ImageBytes;

        public BitmapSource CustomerImage
        {
            get
            {
                if (ImageBytes == null || ImageBytes.Length == 0) return null;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = new MemoryStream(ImageBytes);
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public UploadImage(Customer customer)
        {
            Customer = customer;
            InitializeComponent();
            NewPathText = UIControls.Localization.Resources.ClickToSelectFile;
            ImageBytes = Customer.Image;
            DataContext = this;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.UpdateCustomerImage(Customer.Id, ImageBytes);
                DialogResult = true;
                Close();
            }
            catch (ActionNotSupportedException)
            {
                MessageBox.Show("Необходимо обновление регионального сервера!");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };


            if (dlg.ShowDialog() ?? false)
            {
                NewPathText = dlg.FileName;
                var file = new FileInfo(dlg.FileName);
                using (var fs = file.OpenRead())
                {
                    ImageBytes = Infrastructure.Imaging.StoreImageWithResize(fs, 300, 300);
                    OnPropertyChanged("CustomerImage");
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _context.UpdateCustomerImage(Customer.Id, null);
            DialogResult = true;
            Close();
        }
    }
}
