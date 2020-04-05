using System;
using System.Runtime.Serialization;

namespace GameSessionEstablishmentService
{
    [DataContract]
    public class BlackTurn : ITurnState
    {
        public BlackTurn(GameSession game)
        {
            game.Description.TurnDescription = "Black";
        }

        public OperationResult Move(GameSession game, Player player, string originalPosition, string newPosition)
        {
            var move = new ChessDotNet.Move(originalPosition, newPosition, ChessDotNet.Player.Black, 'Q');

            if (game.Description.BlackPlayer == player && game.ChessLogicCore.IsValidMove(move))
            {
                try
                {
                    game.Description.WhitePlayer.Callback.OnMoved(originalPosition, newPosition);
                }
                catch (Exception) { }
                game.Turn = new WhiteTurn(game);
                game.ChessLogicCore.MakeMove(move, true);

                if (game.ChessLogicCore.IsWinner(ChessDotNet.Player.Black))
                {
                    game.SetBlackWin();
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
