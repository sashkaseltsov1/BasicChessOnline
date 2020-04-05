using ChessClient.Client;
using ChessClient.Modules.FindGame.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ChessClient.Modules.CreateGame.ViewModels
{
    class CreateGameViewModel:BindableBase
    {
        public DelegateCommand BackToFindGameCommand { get; set; }

        public DelegateCommand CreateNewGameCommand { get; set; }

        public bool HasPassword
        {
            get { return _hasPassword; }
            set
            {
                SetProperty(ref _hasPassword, value);
                if (_hasPassword) PasswordVisibility = "Visible"; else { PasswordVisibility = "Collapsed"; Password = ""; };
            }
        }

        public string PasswordVisibility
        {
            get { return _passwordVisibility; }
            set { SetProperty(ref _passwordVisibility, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public string GameName
        {
            get { return _gameName; }
            set { SetProperty(ref _gameName, value); }
        }

        public bool BlackSide
        {
            get { return _blackSide; }
            set { SetProperty(ref _blackSide, value); }
        }

        public bool WhiteSide
        {
            get { return _whiteSide; }
            set { SetProperty(ref _whiteSide, value); }
        }

        private readonly IRegionManager _manager;

        private bool _hasPassword=false;

        private string _passwordVisibility= "Collapsed";

        private string _password;

        private string _gameName;

        private bool _blackSide=false;

        private bool _whiteSide=true;

        public CreateGameViewModel(IRegionManager manager)
        {
            _manager = manager;

            BackToFindGameCommand = new DelegateCommand(()=> 
            {
                ClearFields();
                _manager.RequestNavigate("MainRegion", "FindGame");
            });

            CreateNewGameCommand = new DelegateCommand(CreateNewGame);

            TcpChessClient.getInstance().ConnectedToTheGame += (gameDescription) =>
            {
                var parameters = new NavigationParameters();
                parameters.Add("CurrentGame", new GameDesc(gameDescription));
                _manager.RequestNavigate("MainRegion", "Game", parameters);
            };
        }

        private void CreateNewGame()
        {
            TcpChessClient.getInstance().CreateNewGame(
                    GameName = string.IsNullOrWhiteSpace(GameName) ? "Unnamed" : GameName,
                    WhiteSide == true ? ChessDotNet.Player.White : ChessDotNet.Player.Black,
                    Password);
            ClearFields();
        }

        private void ClearFields()
        {
            GameName = "";
            HasPassword = false;
            WhiteSide = true;
            BlackSide = false;
        }
    }
}
