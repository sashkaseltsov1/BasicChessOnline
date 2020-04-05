using ChessClient.Client;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Text.RegularExpressions;

namespace ChessClient.Modules.MainMenu.LeftArea.ViewModels
{
    class ProfileViewModel:BindableBase, INavigationAware
    {
        public DelegateCommand BackToMenuCommand { get; private set; }

        public DelegateCommand ChangeUsernameCommand { get; private set; }

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public int? Rating
        {
            get { return _rating; }
            set { SetProperty(ref _rating, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public string NewUsername
        {
            get { return _newUsername; }
            set { SetProperty(ref _newUsername, value); }
        }

        private readonly IRegionManager _manager;

        private string _username;

        private int? _rating;

        private string _password;

        private string _newUsername;

        public ProfileViewModel(IRegionManager manager)
        {
            _manager = manager;
            Username = TcpChessClient.getInstance().Username;

            TcpChessClient.getInstance().UsernameChanged += () => Username = TcpChessClient.getInstance().Username;

            BackToMenuCommand = new DelegateCommand(()=> 
            {
                ClearFields();
                _manager.RequestNavigate("LeftRegion", "Menu");
            });

            ChangeUsernameCommand = new DelegateCommand(ChangeUsername);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Rating = navigationContext.Parameters["Rating"] as int?;

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        private void ChangeUsername()
        {
            if (!string.IsNullOrWhiteSpace(NewUsername) && !string.IsNullOrWhiteSpace(Password))
            {
                string usernamePattern = "^([А-ЯЁа-яё0-9a-zA-Z_]{4,23})$";
                string passwordPattern = "^([А-ЯЁа-яё0-9a-zA-Z_!*#&?%^]{4,23})$";
                if (Regex.IsMatch(NewUsername, usernamePattern) && Regex.IsMatch(Password, passwordPattern))
                {
                    TcpChessClient.getInstance().ChangeUsername(NewUsername, Password);
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
            NewUsername = "";
            Password = "";
        }
    }
}
