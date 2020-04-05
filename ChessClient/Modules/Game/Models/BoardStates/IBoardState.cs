
namespace ChessClient.Modules.Game.Models.BoardStates
{
    interface IBoardState
    {
        void Move(Board board, string position);
    }
}
