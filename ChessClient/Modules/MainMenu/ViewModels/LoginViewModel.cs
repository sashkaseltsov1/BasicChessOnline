using ChessClient.Client;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Text.RegularExpressions;

namespace ChessClient.Modules.MainMenu.LeftArea.ViewModels
{
    class LoginViewModel:BindableBase
    {
        public DelegateCommand BackToMenuCommand { get; private set; }

        public DelegateCommand LoginCommand { get; private set; }

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

        private readonly IRegionManager _manager;

        private string _password;

        private string _username;

        public LoginViewModel(IRegionManager manager)
        {
            _manager = manager;

            BackToMenuCommand = new DelegateCommand(()=> 
            {
                ClearFields();
                _manager.RequestNavigate("LeftRegion", "Menu");
            });

            LoginCommand = new DelegateCommand(Login);
        }

        private void Login()
        {
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                string usernamePattern = "^([А-ЯЁа-яё0-9a-zA-Z_]{4,23})$";
                string passwordPattern = "^([А-ЯЁа-яё0-9a-zA-Z_!*#&?%^]{4,23})$";
                if (Regex.IsMatch(Username, usernamePattern) && Regex.IsMatch(Password, passwordPattern))
                {
                    TcpChessClient.getInstance().LoginUser(Username, Password);
                    ClearFields();
                }
                else
                {
                    string message = "Данные должны содержать от 4 до 23 символов, не содержать пробелов и запрещенных символов";
                    TcpChessClient.getInstance().ShowInfo(message);
                }
            }
        }

        private void ClearFields()
        {
            Username = "";
            Password = "";
        }
    }
}
