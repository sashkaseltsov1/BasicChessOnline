using System.Collections.Generic;
using System.ServiceModel;

namespace GameSessionEstablishmentService
{
    [ServiceContract(CallbackContract =typeof(IGameSessionEstablishmentCallback))]
    public interface IGameSessionEstablishment
    {
        [OperationContract(IsOneWay =true)]
        void ConnectToTheGame(int gameId, string username, string password, string gamePassword = null);

        [OperationContract(IsOneWay = true)]
        void DisconnectFromTheGame();

        [OperationContract(IsOneWay = true)]
        void CreateNewGame(string username, string password, string gamename, ChessDotNet.Player side, string gamePassword = null);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string message);

        [OperationContract(IsOneWay = true)]
        void Move(string originalPosition, string newPosition);

        [OperationContract(IsOneWay = true)]
        void GetGames();

        [OperationContract(IsOneWay = true)]
        void SignupUser(string username, string password, string confirmedPassword);

        [OperationContract(IsOneWay = true)]
        void ChangeUsername(string username, string password, string newUsername);

        [OperationContract(IsOneWay = true)]
        void LoginUser(string username, string password);

        [OperationContract(IsOneWay = true)]
        void SystemRequestForGameMaintenance();
    }

    public interface IGameSessionEstablishmentCallback
    {
        [OperationContract(IsOneWay = true)]
        void CallbackInfo(OperationResult result);

        [OperationContract(IsOneWay = true)]
        void SuccesfullConnectionToTheGame(GameDescription gameDescription);

        [OperationContract(IsOneWay = true)]
        void UsernameChanged(string newUsername);

        [OperationContract(IsOneWay = true)]
        void OnMoved(string originalPosition, string newPosition);

        [OperationContract(IsOneWay = true)]
        void GamesGetted(List <GameDescription> games);

        [OperationContract(IsOneWay = true)]
        void SuccesfullAuthentificate(string username, string password, int rating);

        [OperationContract(IsOneWay = true)]
        void GameUpdated(GameDescription gameDescription);

        [OperationContract(IsOneWay = true)]
        void OnMessage(string playerName, string message);

    }
}
