using ChessClient.Client;
using ChessClient.Modules.EventAgregator;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ChessClient.Modules.FindGame.ViewModels
{
    class FindGameViewModel:BindableBase
    {
        public DelegateCommand BackToMainMenuCommand { get; private set; }

        public DelegateCommand GoToCreateGameCommand { get; private set; }

        public DelegateCommand ConnectCommand { get; private set; }

        public DelegateCommand UpdateCommand { get; private set; }

        public int? SelectedIndex
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }

        public string SearchField
        {
            get { return _searchField; }
            set
            {
                SetProperty(ref _searchField, value);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var NewGames = _games.Where(game => game.Name.Contains(value));
                    FilteredGames = new BindingList<GameDesc>(NewGames.ToList());
                }
                else
                {
                    FilteredGames = new BindingList<GameDesc>(_games);
                }
            }
        }

        public BindingList<GameDesc> FilteredGames
        {
            get { return _filteredGames; }
            set { SetProperty(ref _filteredGames, value); }
        }

        private readonly IEventAggregator _ea;

        private readonly IRegionManager _manager;

        private int? _index;

        private int IdLockedGame;

        private string _searchField;

        private List<GameDesc> _games;

        private BindingList<GameDesc> _filteredGames;

        public FindGameViewModel(IRegionManager manager, IEventAggregator ea)
        {
            _ea = ea;
            _manager = manager;
            FilteredGames = new BindingList<GameDesc>();
            _games = new List<GameDesc>();

            _ea.GetEvent<PasswordSentEvent>().Subscribe((password)=> 
            {
                ConnectToTheGame(IdLockedGame, password);
            });
            
            BackToMainMenuCommand = new DelegateCommand(()=> 
            {
                SearchField = "";
                _manager.RequestNavigate("MainRegion", "MainMenu");
            });

            GoToCreateGameCommand = new DelegateCommand(() =>
            {
                SearchField = "";
                _manager.RequestNavigate("MainRegion", "CreateGame");
            });

            UpdateCommand = new DelegateCommand(()=> 
            {
                SearchField = "";
                TcpChessClient.getInstance().GetGames();
            });

            ConnectCommand = new DelegateCommand(Connect);

            TcpChessClient.getInstance().GamesRecieved += (games) => 
            {
                var newGames = new List<GameDesc>();
                foreach (var game in games)
                {
                    newGames.Add(new GameDesc(game));
                }
                _games = new List<GameDesc>(newGames);
                FilteredGames = new BindingList<GameDesc>(_games);
            };
        }

        public void ConnectToTheGame()
        {
            _manager.RequestNavigate("MainRegion", "Game");
        }

        private void ConnectToTheGame(int gameId, string gamePassword)
        {
            SearchField = "";
            TcpChessClient.getInstance().ConnectToTheGame(gameId, gamePassword);
        }

        private void Connect()
        {
            var game = FilteredGames.ElementAt(SelectedIndex.Value);
            if (game.HasPassword)
            {
                IdLockedGame = game.Id;
                _ea.GetEvent<GameLockedEvent>().Publish();
            }
            else
            {
                ConnectToTheGame(game.Id, null);
            }
        }
    }
}
