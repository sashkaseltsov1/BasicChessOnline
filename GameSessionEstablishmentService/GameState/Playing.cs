using System;

namespace GameSessionEstablishmentService
{
    class Playing : IGameState
    {
        public Playing(GameSession game)
        {
            game.Description.StateDerscription = "Playing";
        }

        public OperationResult ConnectPlayer(GameSession game, Player player, string password = null)
        {
            return new OperationResult(false, "Игра " + game.Description.Id + " уже началась.");
        }

        public OperationResult DisconnectPlayer(GameSession game, Player player)
        {
            if(game.Description.WhitePlayer == player)
            {
                game.SetBlackWin();
                game.Description.WhitePlayer = null;
                try
                {
                    game.Description.BlackPlayer.Callback.GameUpdated(game.Description);
                }
                catch (Exception) { }
                return new OperationResult(true, "");
            }
            if (game.Description.BlackPlayer == player)
            {
                game.SetWhiteWin();
                game.Description.BlackPlayer = null;
                try
                {
                    game.Description.WhitePlayer.Callback.GameUpdated(game.Description);
                }
                catch (Exception) { }
                return new OperationResult(true, "");
            }
            return new OperationResult(false, "This game does not contain player " + player.Name);
        }

        public OperationResult Move(GameSession game, Player player, string originalPosition, string newPosition)
        {
            return game.Turn.Move(game, player, originalPosition, newPosition);
        }
    }
}
