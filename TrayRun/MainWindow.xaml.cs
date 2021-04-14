using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using TrayRun.Utils;

namespace TrayRun
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
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

        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private Dictionary<string, Icon> IconHandles = null;
        private readonly ObservableCollection<ServiceModel> _services;


        public MainWindow()
        {
            InitializeComponent();
            ConfigHelper.CheckSettingsFolderExists();
            _services = ConfigHelper<ObservableCollection<ServiceModel>>.GetConfig();

            SetWindowPos();
        }

        private void SetWindowPos()
        {
            // Get desktop size to position the window next to the tray
            var xPos = SystemParameters.PrimaryScreenWidth - this.Width;
            var yPos = SystemParameters.PrimaryScreenHeight - this.Height - 40;
            this.Left = xPos;
            this.Top = yPos;
        }

        protected override void OnInitialized(EventArgs e)
        {
            IconHandles = new Dictionary<string, Icon>
            {
                { "QuickLaunch", Properties.Resources.cogs_icon }
            };

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(NotifyIcon_Clicked);
            notifyIcon.Icon = IconHandles["QuickLaunch"];

            base.OnInitialized(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            notifyIcon.Visible = true;
            ServiceList.ItemsSource = _services;
        }

        private void NotifyIcon_Clicked(object sender, EventArgs e)
        {
            ShowTrayMenu();
        }

        private void ShowTrayMenu()
        {
            if (this.IsVisible)
            {
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                SetWindowPos();
                this.Show();
            }
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new EditService(this, null);
            addWindow.Show();
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = ServiceList.SelectedItem;
            if (item is null) return;
            _services.Remove(item as ServiceModel);
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            // Remove the tray icon
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            // Save settings
            ConfigHelper<ObservableCollection<ServiceModel>>.SaveConfig(_services);
        }

        public void AddService(ServiceModel service)
        {
            _services.Add(service);
        }

        public void Start_Clicked(object sender, RoutedEventArgs e)
        {
            var curItem = ServiceList.ContainerFromElement(sender as Button) as ListBoxItem;
            var service = curItem.DataContext as ServiceModel;
            service.RunStartCommand();
        }

        public void Stop_Clicked(object sender, RoutedEventArgs e)
        {
            var curItem = ServiceList.ContainerFromElement(sender as Button) as ListBoxItem;
            var service = curItem.DataContext as ServiceModel;
            service.RunStopCommand();
        }
    }
}
