using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayRun.Utils;

namespace TrayRun
{
    public class ServiceModel : Bindable
    {
        private bool _status;
        public bool Status {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _title;
        public string Title {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _startCommand;
        public string StartCommand {
            get => _startCommand;
            set => SetProperty(ref _startCommand, value);
        }

        private string _stopCommand;
        public string StopCommand {
            get => _stopCommand;
            set => SetProperty(ref _stopCommand, value);
        }

        private bool _isLauncher;
        public bool IsLauncher {
            get => _isLauncher;
            set => SetProperty(ref _isLauncher, value);
        }

        private bool _isRunning;
        public bool IsRunning {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public void RunStartCommand()
        {
            if (IsLauncher)
            {
                Process.Start(StartCommand);
                return;
            }

            var info = new ProcessStartInfo("cmd.exe")
            {
                Arguments = "/C " + StartCommand,
                WindowStyle = ProcessWindowStyle.Minimized
            };
            Process.Start(info);
            IsRunning = true;
        }

        public void RunStopCommand()
        {
            var info = new ProcessStartInfo("cmd.exe")
            {
                Arguments = "/C " + StopCommand,
                WindowStyle = ProcessWindowStyle.Minimized
            };
            Process.Start(info);
            IsRunning = false;
        }
    }
}
