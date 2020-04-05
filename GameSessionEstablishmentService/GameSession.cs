using System;
using System.Runtime.Remoting.Contexts;

namespace GameSessionEstablishmentService
{
    [Synchronization()]
    public class GameSession: ContextBoundObject
    {
        public delegate void GameHandler(GameSession game);

        public event GameHandler NotifyAboutDeletingTheGame;

        public GameDescription Description { get; set; }

        public IGameState State { get; set; }

        public ITurnState Turn { get; set; }

        public ChessDotNet.ChessGame ChessLogicCore { get; set; }

        public GameSession(int id, string name, Player player, ChessDotNet.Player side, string gamePassword = null)
        {
            if (string.IsNullOrWhiteSpace(name) | player == null) throw new ArgumentNullException();

            Description = new GameDescription()
            {
                Id=id,
                Name=name,
                Password = gamePassword 
            };
            if (side == ChessDotNet.Player.Black) Description.BlackPlayer = player; else Description.WhitePlayer = player;
            State = new Waiting(this);
            Turn = new WhiteTurn(this);
            ChessLogicCore = new ChessDotNet.ChessGame();
        }

        public OperationResult ConnectPlayer(Player player, string gamePassword = null)
        {
            if (player == null) throw new ArgumentNullException();

            return State.ConnectPlayer(this, player, gamePassword);
        }

        public OperationResult DisconnectPlayer(Player player)
        {
            if (player == null) throw new ArgumentNullException();

            OperationResult result = State.DisconnectPlayer(this, player);
            if(Description.WhitePlayer==null && Description.BlackPlayer==null)
            {
                NotifyAboutDeletingTheGame(this);
            }
            return result;
        }

        public void CalculateRates()
        {
            if(Description.BlackPlayer !=null & Description.WhitePlayer !=null)
            {
                Description.BlackPlayer.CalculateRates(Description.WhitePlayer);
                Description.WhitePlayer.CalculateRates(Description.BlackPlayer);
            }
        }

        public OperationResult Move(Player player, string originalPosition, string newPosition)
        {
            if (player == null) throw new ArgumentNullException();

            return State.Move(this, player, originalPosition, newPosition);
        }

        public void SetBlackWin()
        {
            State = new Ended(this);
            Description.BlackPlayer.UpdateTotalScore(Description.BlackPlayer.RatingForWin);
            Description.WhitePlayer.UpdateTotalScore(Description.WhitePlayer.RatingForLose);
            Description.Result = "Черные победили!";
        }

        public void SetWhiteWin()
        {
            State = new Ended(this);
            Description.BlackPlayer.UpdateTotalScore(Description.BlackPlayer.RatingForLose);
            Description.WhitePlayer.UpdateTotalScore(Description.WhitePlayer.RatingForWin);
            Description.Result = "Белые победили!";
        }

        public void SetDraw()
        {
            State = new Ended(this);
            Description.BlackPlayer.UpdateTotalScore(Description.BlackPlayer.RatingForDraw);
            Description.WhitePlayer.UpdateTotalScore(Description.WhitePlayer.RatingForDraw);
            Description.Result = "Ничья!";
        }

        public void SendMessage(Player player, string message)
        {
            if(player==Description.WhitePlayer)
            {
                try
                {
                    Description.BlackPlayer.Callback.OnMessage(player.Name, message);
                }
                catch (Exception){}
            }
            if (player == Description.BlackPlayer)
            {
                try
                {
                    Description.WhitePlayer.Callback.OnMessage(player.Name, message);
                }
                catch (Exception) { }
            }
        }
    }
}
