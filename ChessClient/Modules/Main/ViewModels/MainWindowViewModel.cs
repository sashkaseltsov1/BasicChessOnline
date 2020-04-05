using ChessClient.Client;
using ChessClient.Modules.EventAgregator;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace ChessClient.Main.ViewModels
{
    class MainWindowViewModel:BindableBase
    {
        public DelegateCommand ClosePopupCommand { get; set; }

        public DelegateCommand SendPasswordCommand { get; set; }

        public string LoadingPopupVisibility
        {
            get { return _loadingPopupVisibility; }
            set { SetProperty(ref _loadingPopupVisibility, value); }
        }

        public string CallbackPopupMessage
        {
            get { return _callbackPopupMessage; }
            set { SetProperty(ref _callbackPopupMessage, value); }
        }

        public string BackgroundPopupVisibility
        {
            get { return _backgroundPopupVisibility; }
            set { SetProperty(ref _backgroundPopupVisibility, value); }
        }

        public string CallbackPopupVisibility
        {
            get { return _callbackPopupVisibility; }
            set
            {
                SetProperty(ref _callbackPopupVisibility, value);
                BackgroundPopupVisibility = value;
            }
        }

        public string PasswordPopupVisibility
        {
            get { return _passwordPopupVisibility; }
            set
            {
                SetProperty(ref _passwordPopupVisibility, value);
                BackgroundPopupVisibility = value;
            }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private readonly IEventAggregator _ea;

        private string _loadingPopupVisibility;

        private string _callbackPopupMessage;

        private string _backgroundPopupVisibility;

        private string _callbackPopupVisibility;

        private string _passwordPopupVisibility;

        private string _password;

        public MainWindowViewModel(IEventAggregator ea)
        {
            _ea = ea;
            PasswordPopupVisibility = "Hidden";
            CallbackPopupVisibility = "Hidden";
            LoadingPopupVisibility = "Hidden";

            ClosePopupCommand = new DelegateCommand(() =>
            {
                CallbackPopupVisibility = "Hidden";
                PasswordPopupVisibility = "Hidden";
            });

            SendPasswordCommand = new DelegateCommand(() =>
            {
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    PasswordPopupVisibility = "Hidden";
                    _ea.GetEvent<PasswordSentEvent>().Publish(Password);
                }
            });

            _ea.GetEvent<GameLockedEvent>().Subscribe(() => 
            {
                Password = "";
                PasswordPopupVisibility = "Visible";
            });

            TcpChessClient.getInstance().CallbackInfoRecieved += (result) => 
            {
                CallbackPopupMessage = result.Message;
                CallbackPopupVisibility = "Visible";
            };

            TcpChessClient.getInstance().CommandExecutionStarted += () => LoadingPopupVisibility = "Visible";

            TcpChessClient.getInstance().CommandExecutionStoped += () => LoadingPopupVisibility = "Hidden";
        }
    }
}
