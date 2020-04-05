using ChessClient.Client;
using ChessClient.Modules.EventAgregator;
using ChessClient.Modules.FindGame.ViewModels;
using ChessClient.Modules.Game.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.ComponentModel;

namespace ChessClient.Modules.Game.ViewModels
{
    class ChessBoardViewModel:BindableBase
    {
        public DelegateCommand<string> MoveCommand { get; set; }

        private readonly IEventAggregator _ea;

        public BindingList<Cell> Cells
        {
            get { return _cells; }
            set { SetProperty(ref _cells, value); }
        }

        public BindingList<char> HorizontalIndexes
        {
            get { return _horizontalIndexes; }
            set { SetProperty(ref _horizontalIndexes, value); }
        }

        public BindingList<char> VerticalIndexes
        {
            get { return _verticalIndexes; }
            set { SetProperty(ref _verticalIndexes, value); }
        }

        private Board Board { get; set; }

        private string _state;

        private BindingList<Cell> _cells;

        private BindingList<char> _horizontalIndexes;

        private BindingList<char> _verticalIndexes;

        public ChessBoardViewModel(IEventAggregator ea)
        {
            _ea = ea;

            _ea.GetEvent<GameDescriptionSentEvent>().Subscribe(InitializeBoard);

            TcpChessClient.getInstance().PlayerMoved += (origPos, newPos) => 
            {
                Board.EnemyMoved(origPos, newPos);
            };

            TcpChessClient.getInstance().GameStateChanged += (gameDesc) => { _state = gameDesc.StateDerscription; };
        }

        private void InitializeBoard(GameDesc desc)
        {
            _state = desc.State;
            ChessDotNet.Player side =
            TcpChessClient.getInstance().Username == desc.WhiteUsername ? 
            ChessDotNet.Player.White : ChessDotNet.Player.Black;
            Board = new Board(side);
            Cells = Board.Cells;
            HorizontalIndexes = Board.HorizontalIndexes;
            VerticalIndexes = Board.VerticalIndexes;

            MoveCommand = new DelegateCommand<string>((move)=> { if (_state == "Playing") Board.MakeMove(move); });
        }
    }
}
