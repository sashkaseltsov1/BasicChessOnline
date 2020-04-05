
namespace GameSessionEstablishmentService
{
    class Ended : IGameState
    {
        public Ended(GameSession game)
        {
            game.Description.StateDerscription = "Ended";
        }

        public OperationResult ConnectPlayer(GameSession game, Player player, string password = null)
        {
            return new OperationResult(false, "Игра " + game.Description.Id + " уже закончилась.");
        }

        public OperationResult DisconnectPlayer(GameSession game, Player player)
        {
            if (game.Description.WhitePlayer == player)
            {
                game.Description.WhitePlayer = null;
                return new OperationResult(true, "");
            }
            if (game.Description.BlackPlayer == player)
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
