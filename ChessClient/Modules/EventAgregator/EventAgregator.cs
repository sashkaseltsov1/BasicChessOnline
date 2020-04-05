using ChessClient.Modules.FindGame.ViewModels;
using Prism.Events;

namespace ChessClient.Modules.EventAgregator
{
    class GameDescriptionSentEvent : PubSubEvent<GameDesc> { }

    class GameLockedEvent : PubSubEvent { }

    class PasswordSentEvent : PubSubEvent<string> { }
}
