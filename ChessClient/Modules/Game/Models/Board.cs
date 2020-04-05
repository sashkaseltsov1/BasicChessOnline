using ChessClient.Modules.Game.Models.BoardStates;
using ChessClient.Modules.Game.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ChessClient.Modules.Game.Models
{
    class Board :BindableBase
    {
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

        public ChessDotNet.Player Side { get; set; }

        public ChessDotNet.ChessGame ChessLogic { get; set; }

        public IBoardState State { get; set; }

        public Cell SelectedCell { get; set; }

        private BindingList<Cell> _cells;

        private BindingList<char> _horizontalIndexes;

        private BindingList<char> _verticalIndexes;

        public Board(ChessDotNet.Player side)
        {
            Cells = new BindingList<Cell>();
            InitializeCells(side);
            InitializeFigures(side);
            State = new PieceDroped();
            Side = side;
            ChessLogic = new ChessDotNet.ChessGame();
        }

        public void MakeMove(string move)
        {
            State.Move(this, move);
        }

        public void EnemyMoved(string originalPosition, string newPosition)
        {
            ChessDotNet.Player enemySide = Side == ChessDotNet.Player.White ? ChessDotNet.Player.Black : ChessDotNet.Player.White;
            SelectedCell = Cells.Single(item => item.Position == originalPosition);
            var move = new ChessDotNet.Move(originalPosition, newPosition, enemySide, 'Q');
            var moveType = ChessLogic.MakeMove(move, true);
            UpdateSourceIfPromotion(moveType, newPosition, enemySide);
            SelectedCell.Source = null;
            UpdateSourceIfEnPassant(moveType, newPosition, enemySide);
            UpdateSourceIfCastling(moveType, newPosition);
        }

        public void UpdateSourceIfPromotion(ChessDotNet.MoveType moveType, string newPosition, ChessDotNet.Player side)
        {
            if (moveType == (ChessDotNet.MoveType.Capture|ChessDotNet.MoveType.Promotion | ChessDotNet.MoveType.Move))
            {
                string sourceForPromotion = side == ChessDotNet.Player.White ? "/Resources/Pieces/wQ.png" : "/Resources/Pieces/bQ.png";
                Cells.Single(item => item.Position == newPosition).Source = sourceForPromotion;
            }
            else
            {
                Cells.Single(item => item.Position == newPosition).Source = SelectedCell.Source;
            }
        }

        public void UpdateSourceIfEnPassant(ChessDotNet.MoveType moveType, string newPosition, ChessDotNet.Player side)
        {
            if (moveType == (ChessDotNet.MoveType.Move | ChessDotNet.MoveType.EnPassant | ChessDotNet.MoveType.Capture))
            {
                char c = newPosition.ElementAt(0);
                int verticalIndex = (int)Char.GetNumericValue(newPosition.ElementAt(1));
                if (side == ChessDotNet.Player.White) verticalIndex--; else verticalIndex++;
                newPosition = c.ToString() + verticalIndex.ToString();
                Cells.Single(item => item.Position == newPosition).Source = null;
            }
        }

        public void UpdateSourceIfCastling(ChessDotNet.MoveType moveType, string newPosition)
        {
            if (moveType == (ChessDotNet.MoveType.Castling | ChessDotNet.MoveType.Move))
            {
                char c = newPosition.ElementAt(0);
                string rockPosition;
                string newRockPosition;
                if (c == 'G')
                {
                    rockPosition = "H" + newPosition.ElementAt(1).ToString();
                    newRockPosition = "F" + newPosition.ElementAt(1).ToString();
                }
                else
                {
                    rockPosition = "A" + newPosition.ElementAt(1).ToString();
                    newRockPosition = "D" + newPosition.ElementAt(1).ToString();
                }
                var cellWithRock = Cells.Single(item => item.Position == rockPosition);
                Cells.Single(item => item.Position == newRockPosition).Source = cellWithRock.Source;
                cellWithRock.Source = null;
            }
        }

        private void InitializeCells(ChessDotNet.Player side)
        {
            InitializeColor();
            InitializeIndexes(side);
            InitializeNames();
        }

        private void InitializeColor()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i % 2 == 0)
                    {
                        Cells.Add(new Cell("#d0c6bd"));
                        Cells.Add(new Cell("#2d2d30"));
                    }
                    else
                    {
                        Cells.Add(new Cell("#2d2d30"));
                        Cells.Add(new Cell("#d0c6bd"));
                    }
                }
            }
        }

        private void InitializeIndexes(ChessDotNet.Player side)
        {
            List<char> hIndexes = new List<char>()
            {
                'A','B','C','D','E','F','G','H'
            };

            List<char> vIndexes = new List<char>()
            {
                '8','7','6','5','4','3','2','1'
            };

            if (side == ChessDotNet.Player.Black)
            {
                hIndexes.Reverse();
                vIndexes.Reverse();
            }

            HorizontalIndexes = new BindingList<char>(hIndexes);

            VerticalIndexes = new BindingList<char>(vIndexes);
        }

        private void InitializeNames()
        {
            int cellIndex = 0;
            for (int v = 0; v < VerticalIndexes.Count; v++)
            {
                for (int h = 0; h < HorizontalIndexes.Count; h++)
                {
                    Cells[cellIndex].Position = HorizontalIndexes[h].ToString() + VerticalIndexes[v].ToString();
                    cellIndex++;
                }
            }
        }

        private void InitializeFigures(ChessDotNet.Player side)
        {
            List<string> whitePieces = new List<string>()
                {
                    "wR", "wN", "wB", "wK", "wQ", "wB", "wN", "wR",
                    "wP", "wP", "wP", "wP", "wP", "wP", "wP", "wP"
                };

            List<string> blackPieces = new List<string>()
                {
                    "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR",
                    "bP", "bP", "bP", "bP", "bP", "bP", "bP", "bP"
                };

            if(side==ChessDotNet.Player.Black)
            {
                
                List<string> tempPieces = new List<string>(whitePieces);
                whitePieces = new List<string>(blackPieces);
                blackPieces = new List<string>(tempPieces);
            }

            int count = 0;

            for (int i = Cells.Count-1; i > Cells.Count-whitePieces.Count-1; i--)
            {
                Cells[i].Source = "/Resources/Pieces/" + whitePieces[count] + ".png";
                count++;
            }

            count = 0;

            for (int i = 0; i < blackPieces.Count; i++)
            {
                Cells[i].Source = "/Resources/Pieces/" + blackPieces[count] + ".png";
                count++;
            }
        }
    }
}
