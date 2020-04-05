using ChessClient.Client;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Text.RegularExpressions;

namespace ChessClient.Modules.MainMenu.LeftArea.ViewModels
{
    class SignupViewModel:BindableBase
    {
        public DelegateCommand BackToMenuCommand { get; private set; }

        public DelegateCommand SignupCommand { get; private set; }

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public string ConfirmedPassword
        {
            get { return _confirmedPassword; }
            set { SetProperty(ref _confirmedPassword, value); }
        }

        private readonly IRegionManager _manager;

        private string _password;

        private string _username;

        private string _confirmedPassword;

        public SignupViewModel(IRegionManager manager)
        {
            _manager = manager;

            BackToMenuCommand = new DelegateCommand(()=> 
            {
                ClearFields();
                _manager.RequestNavigate("LeftRegion", "Menu");
            });

            SignupCommand = new DelegateCommand(Signup);
        }

        private void ClearFields()
        {
            Username = "";
            Password = "";
            ConfirmedPassword = "";
        }

        private void Signup()
        {
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ConfirmedPassword))
            {
                string usernamePattern = "^([А-ЯЁа-яё0-9a-zA-Z_]{4,23})$";
                string passwordPattern = "^([А-ЯЁа-яё0-9a-zA-Z_!*#&?%^]{4,23})$";
                if (Regex.IsMatch(Username, usernamePattern) && Regex.IsMatch(Password, passwordPattern) && Regex.IsMatch(ConfirmedPassword, passwordPattern))
                {
                    TcpChessClient.getInstance().SignupUser(Username, Password, ConfirmedPassword);
                    ClearFields();
                }
                else
                {
                    string message = "Данные должны содержать от 4 до 23 символов, не содержать пробелов и запрещенных символов";
                    TcpChessClient.getInstance().ShowInfo(message);
                }
            }
        }
    }
}
