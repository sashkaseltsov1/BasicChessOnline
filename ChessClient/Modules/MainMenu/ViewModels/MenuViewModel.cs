using ChessClient.Client;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ChessClient.Modules.MainMenu.LeftArea.ViewModels
{
    class MenuViewModel:BindableBase
    {
        public DelegateCommand GoToFindGameCommand { get; private set; }

        public DelegateCommand GoToProfileCommand { get; private set; }

        public DelegateCommand GoToSignupCommand { get; private set; }

        public DelegateCommand GoToLoginCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public bool FindGameButtonIsEnabled
        {
            get { return _findGameButtonIsEnabled; }
            set { SetProperty( ref _findGameButtonIsEnabled, value); }
        }

        public bool ProfileButtonIsEnabled
        {
            get { return _profileButtonIsEnabled; }
            set { SetProperty(ref _profileButtonIsEnabled, value); }
        }

        public bool SignupButtonIsEnabled
        {
            get { return _signupButtonIsEnabled; }
            set { SetProperty(ref _signupButtonIsEnabled, value); }
        }

        public bool LoginButtonIsEnabled
        {
            get { return _loginButtonIsEnabled; }
            set { SetProperty(ref _loginButtonIsEnabled, value); }
        }

        public bool ExitButtonIsEnabled
        {
            get { return _exitButtonIsEnabled; }
            set { SetProperty(ref _exitButtonIsEnabled, value); }
        }

        private readonly IRegionManager _manager;

        private bool _findGameButtonIsEnabled;

        private bool _profileButtonIsEnabled;

        private bool _signupButtonIsEnabled;

        private bool _loginButtonIsEnabled;

        private bool _exitButtonIsEnabled;

        public MenuViewModel(IRegionManager manager)
        {
            _manager = manager;

            GoToFindGameCommand = new DelegateCommand(() =>
            {
                TcpChessClient.getInstance().GetGames();
                _manager.RequestNavigate("MainRegion", "FindGame");
            });

            GoToProfileCommand = new DelegateCommand(() =>
            {
                var parameters = new NavigationParameters();
                parameters.Add("Rating", TcpChessClient.getInstance().Rating);
                _manager.RequestNavigate("LeftRegion", "Profile", parameters);

            });

            GoToSignupCommand = new DelegateCommand(() => _manager.RequestNavigate("LeftRegion", "Signup"));

            GoToLoginCommand = new DelegateCommand(() => _manager.RequestNavigate("LeftRegion", "Login"));

            ExitCommand = new DelegateCommand(SetNotAuthentificatedState);

            TcpChessClient.getInstance().Authentificated += () =>
            {
                SetAuthentificatedState();
                _manager.RequestNavigate("LeftRegion", "Menu");
            };

            SetNotAuthentificatedState();
        }

        private void SetAuthentificatedState()
        {
            FindGameButtonIsEnabled = true;
            ProfileButtonIsEnabled = true;
            SignupButtonIsEnabled = false;
            LoginButtonIsEnabled = false;
            ExitButtonIsEnabled = true;
        }

        private void SetNotAuthentificatedState()
        {
            FindGameButtonIsEnabled = false;
            ProfileButtonIsEnabled = false;
            SignupButtonIsEnabled = true;
            LoginButtonIsEnabled = true;
            ExitButtonIsEnabled = false;
        }
    }
}
