using System.Linq;

namespace ChessClient.Modules.Game.Models.BoardStates
{
    class PieceDroped : IBoardState
    {
        public void Move(Board board, string position)
        {
           if( board.ChessLogic.HasAnyValidMoves(new ChessDotNet.Position(position)) && board.ChessLogic.WhoseTurn==board.Side )
           {
                var validMoves = board.ChessLogic.GetValidMoves((new ChessDotNet.Position(position)));
                foreach (var move in validMoves)
                {
                    board.Cells.SingleOrDefault(item => item.Position == move.NewPosition.ToString()).ValidCell = "Visible";
                }
                board.SelectedCell = board.Cells.Single(item => item.Position == position);
                board.State = new PieceCaptured();
           }
        }
    }
}
