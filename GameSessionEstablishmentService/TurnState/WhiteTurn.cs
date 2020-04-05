using System;
using System.Runtime.Serialization;

namespace GameSessionEstablishmentService
{
    [DataContract]
    public class WhiteTurn : ITurnState
    {
        public WhiteTurn(GameSession game)
        {
            game.Description.TurnDescription = "White";
        }

        public OperationResult Move(GameSession game, Player player, string originalPosition, string newPosition)
        {
            var move = new ChessDotNet.Move(originalPosition, newPosition, ChessDotNet.Player.White, 'Q');

            if (game.Description.WhitePlayer == player && game.ChessLogicCore.IsValidMove(move))
            {
                try
                {
                    game.Description.BlackPlayer.Callback.OnMoved(originalPosition ,newPosition);
                }
                catch (Exception) { }
                game.Turn = new BlackTurn(game);
                game.ChessLogicCore.MakeMove(move, true);

                if (game.ChessLogicCore.IsWinner(ChessDotNet.Player.White))
                {
                    game.SetWhiteWin();
                    NotifyPlayersAboutEndGame(game.Description);
                }
                if (game.ChessLogicCore.IsDraw())
                {
                    game.SetDraw();
                    NotifyPlayersAboutEndGame(game.Description);
                }

                return new OperationResult(true, "");
            }
            return new OperationResult(false, "");
        }

        private void NotifyPlayersAboutEndGame(GameDescription gameDesc)
        {
            try
            {
                gameDesc.WhitePlayer.Callback.GameUpdated(gameDesc);
            }
            catch (Exception) { }
            try
            {
                gameDesc.BlackPlayer.Callback.GameUpdated(gameDesc);
            }
            catch (Exception) { }
        }
    }
}
