using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSessionEstablishmentService
{
    public interface ITurnState
    {
        OperationResult Move(GameSession game, Player player, string originalPosition, string newPosition);
    }
}
