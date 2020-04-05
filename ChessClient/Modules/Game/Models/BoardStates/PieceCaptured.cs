using ChessClient.Client;

namespace ChessClient.Modules.Game.Models.BoardStates
{
    class PieceCaptured:IBoardState
    {
        public void Move(Board board, string position)
        {
            var move = new ChessDotNet.Move(board.SelectedCell.Position, position, board.Side, 'Q');

            foreach (var item in board.Cells)
            {
                item.ValidCell = "Hidden";
            }

            if (board.ChessLogic.IsValidMove(move))
            {
                var moveType = board.ChessLogic.MakeMove(move, true);
                board.UpdateSourceIfPromotion(moveType, position, board.Side);
                board.SelectedCell.Source = null;
                board.State = new PieceDroped();
                board.UpdateSourceIfEnPassant(moveType, position, board.Side);
                board.UpdateSourceIfCastling(moveType, position);

                TcpChessClient.getInstance().Move(move.OriginalPosition.ToString(), move.NewPosition.ToString());            
            }
            else
            {
                board.State = new PieceDroped();
                board.State.Move(board, position);
            }
        }
    }
}
