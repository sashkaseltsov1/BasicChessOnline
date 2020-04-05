using ChessClient.Client;
using ChessClient.Modules.EventAgregator;
using ChessClient.Modules.FindGame.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace ChessClient.Modules.Game.ViewModels
{
    class DescriptionViewModel : BindableBase
    {
        public DelegateCommand DisconnectCommand { get; set; }

        public string GameName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public string StateColor
        {
            get { return _stateColor; }
            set { SetProperty(ref _stateColor, value); }
        }

        public string WhitePlayerName
        {
            get { return _whitePlayerName; }
            set { SetProperty(ref _whitePlayerName, value); }
        }

        public string BlackPlayerName
        {
            get { return _blackPlayerName; }
            set { SetProperty(ref _blackPlayerName, value); }
        }

        public int? WhitePlayerRating
        {
            get { return _whiteRating; }
            set { SetProperty(ref _whiteRating, value); }
        }

        public int? BlackPlayerRating
        {
            get { return _blackRating; }
            set { SetProperty(ref _blackRating, value); }
        }

        public int? WhitePlayerRatingForWin
        {
            get { return _whiteRatingForWin; }
            set { SetProperty(ref _whiteRatingForWin, value); }
        }

        public int? WhitePlayerRatingForLose
        {
            get { return _whiteRatingForLose; }
            set { SetProperty(ref _whiteRatingForLose, value); }
        }

        public int? WhitePlayerRatingForDraw
        {
            get { return _whiteRatingForDraw; }
            set { SetProperty(ref _whiteRatingForDraw, value); }
        }

        public int? BlackPlayerRatingForWin
        {
            get { return _blackRatingForWin; }
            set { SetProperty(ref _blackRatingForWin, value); }
        }

        public int? BlackPlayerRatingForLose
        {
            get { return _blackRatingForLose; }
            set { SetProperty(ref _blackRatingForLose, value); }
        }

        public int? BlackPlayerRatingForDraw
        {
            get { return _blackRatingForDraw; }
            set { SetProperty(ref _blackRatingForDraw, value); }
        }

        private readonly IEventAggregator _ea;

        private readonly IRegionManager _manager;

        private string _state;

        private string _stateColor;

        private string _whitePlayerName;

        private string _blackPlayerName;

        private string _name;

        private int? _whiteRating;

        private int? _blackRating;

        private int? _whiteRatingForWin;

        private int? _blackRatingForWin;

        private int? _whiteRatingForLose;

        private int? _blackRatingForLose;

        private int? _whiteRatingForDraw;

        private int? _blackRatingForDraw;

        public DescriptionViewModel(IEventAggregator ea, IRegionManager manager)
        {
            _manager = manager;
            _ea = ea;

            DisconnectCommand = new DelegateCommand(Disconnect);

            _ea.GetEvent<GameDescriptionSentEvent>().Subscribe(InitializeGameDescription);

            TcpChessClient.getInstance().GameStateChanged += (gameDesc) => 
            {
                InitializeGameDescription(new GameDesc(gameDesc));
            };
        }

        private void Disconnect()
        {
            bool isWhitePlayer = WhitePlayerName == TcpChessClient.getInstance().Username ? true : false;
            if (State == "Playing")
            {
                if (isWhitePlayer)
                {
                    TcpChessClient.getInstance().Rating = TcpChessClient.getInstance().Rating + WhitePlayerRatingForLose.Value;
                }
                else
                {
                    TcpChessClient.getInstance().Rating = TcpChessClient.getInstance().Rating + BlackPlayerRatingForLose.Value;
                }
            }
            else
            {
                if (isWhitePlayer)
                {
                    TcpChessClient.getInstance().Rating = WhitePlayerRating.Value;
                }
                else
                {
                    TcpChessClient.getInstance().Rating = BlackPlayerRating.Value;
                }
            }
            TcpChessClient.getInstance().DisconnectFromTheGame();
            _manager.RequestNavigate("MainRegion", "FindGame");
        }

        private void InitializeGameDescription(GameDesc desc)
        {
            GameName = desc.Name;
            State = desc.State;
            StateColor = desc.StateFieldColor;
            WhitePlayerName = desc.WhiteUsername;
            BlackPlayerName = desc.BlackUsername;
            WhitePlayerRating = desc.WhiteRating;
            BlackPlayerRating = desc.BlackRating;
            WhitePlayerRatingForWin = desc.WhiteRatingForWin;
            BlackPlayerRatingForWin = desc.BlackRatingForWin;
            WhitePlayerRatingForLose = desc.WhiteRatingForLose;
            BlackPlayerRatingForLose = desc.BlackRatingForLose;
            WhitePlayerRatingForDraw = desc.WhiteRatingForDraw;
            BlackPlayerRatingForDraw = desc.BlackRatingForDraw;
        }
    }
}
