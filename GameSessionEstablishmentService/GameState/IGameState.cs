
namespace GameSessionEstablishmentService
{
    public interface IGameState
    {
        OperationResult ConnectPlayer(GameSession game, Player player, string password=null);

        OperationResult DisconnectPlayer(GameSession game, Player player);

        OperationResult Move(GameSession game, Player player, string originalPosition, string newPosition);
    }
}
