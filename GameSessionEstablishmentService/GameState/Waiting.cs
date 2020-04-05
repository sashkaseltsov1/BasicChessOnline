using System;

namespace GameSessionEstablishmentService
{
    class Waiting : IGameState
    {
        public Waiting(GameSession game)
        {
            game.Description.StateDerscription = "Waiting";
        }

        public OperationResult ConnectPlayer(GameSession game, Player player, string password = null)
        {
            if (game.Description.Password == password)
            {
                if (game.Description.WhitePlayer == null)
                {
                    game.Description.WhitePlayer = player;
                    game.CalculateRates();
                    game.State = new Playing(game);
                    try
                    {
                        game.Description.BlackPlayer.Callback.GameUpdated(game.Description);
                    }
                    catch (Exception) { }
                    
                    return new OperationResult(true, "");
                }
                else
                if (game.Description.BlackPlayer == null)
                {
                    game.Description.BlackPlayer = player;
                    game.CalculateRates();
                    game.State = new Playing(game);
                    try
                    {
                        game.Description.WhitePlayer.Callback.GameUpdated(game.Description);
                    }
                    catch (Exception) { }
                    return new OperationResult(true, "");
                }
                else
                {
                    return new OperationResult(false, "Игра " + game.Description.Id + " уже началась.");
                }
            }
            return new OperationResult(false, "Неверный пароль.");
        }

        public OperationResult DisconnectPlayer(GameSession game, Player player)
        {
            if(game.Description.WhitePlayer ==player)
            {
                game.Description.WhitePlayer = null;
                return new OperationResult(true, "");
            }
            if(game.Description.BlackPlayer == player)
            {
                game.Description.BlackPlayer = null;
                return new OperationResult(true, "");
            }
            return new OperationResult(false, "");
        }

        public OperationResult Move(GameSession game, Player player, string originalPosition, string newPosition)
        {
            return new OperationResult(false, "");
        }
    }
}
