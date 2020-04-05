using ChessClient.GameSessionEstablishmentServiceRef;

namespace ChessClient.Client
{
    class Callback : GameSessionEstablishmentServiceRef.IGameSessionEstablishmentCallback
    {
        public delegate void CallbackInfoHandler(OperationResult result);

        public delegate void GameDescriptionResultHandler(GameDescription game);

        public delegate void GamesResultHandler(GameDescription [] game);

        public delegate void DoubleParamStringHandler(string str1, string str2);

        public delegate void UsernameHandler(string username);

        public delegate void AuthentificateHandler(string username, string password, int rate);

        public event CallbackInfoHandler CallbackInfoRecieved;

        public event GameDescriptionResultHandler ConnectedToTheGame;

        public event GameDescriptionResultHandler GameStateChanged;

        public event AuthentificateHandler Authentificated;

        public event GamesResultHandler GamesRecieved;

        public event DoubleParamStringHandler MessageRecieved;

        public event DoubleParamStringHandler PlayerMoved;

        public event UsernameHandler UsernameFieldChanged;

        public void CallbackInfo(OperationResult result)
        {
            CallbackInfoRecieved(result);
        }

        public void SuccesfullConnectionToTheGame(GameDescription gameDescription)
        {
            ConnectedToTheGame(gameDescription);
        }

        public void OnMoved(string originalPosition, string newPosition)
        {
            PlayerMoved(originalPosition, newPosition);
        }

        public void GamesGetted(GameDescription[] games)
        {
            GamesRecieved(games);
        }

        public void SuccesfullAuthentificate(string username, string password, int rating)
        {
            Authentificated(username, password, rating);
        }

        public void GameUpdated(GameDescription gameDescription)
        {
            GameStateChanged(gameDescription);
        }

        public void OnMessage(string playerName, string message)
        {
            MessageRecieved(playerName, message);
        }

        public void UsernameChanged(string newUsername)
        {
            UsernameFieldChanged(newUsername);
        }
    }
}
