using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrayRun
{
    /// <summary>
    /// Interaction logic for EditService.xaml
    /// </summary>
    public partial class EditService : INotifyPropertyChanged
    {
        #region Binding Setup
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged == null) return;
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        private ServiceModel _serviceModel;
        public ServiceModel Service {
            get => _serviceModel;
            set => SetProperty(ref _serviceModel, value);
        }

        private MainWindow _parent;

        public EditService(MainWindow parent, ServiceModel existingModel)
        {
            InitializeComponent();
            _parent = parent;

            if (existingModel != null)
            {
                Service = existingModel;
            }
            else
            {
                Service = new ServiceModel
                {
                    Status = false,
                    StartCommand = "",
                    Title = ""
                };
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parent.AddService(Service);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
