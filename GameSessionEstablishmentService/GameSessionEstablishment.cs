using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GameSessionEstablishmentService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,ConcurrencyMode = ConcurrencyMode.Single)]
    public class GameSessionEstablishment : IGameSessionEstablishment
    {
        private static int _gameId=0;

        private static List<GameSession> _games = new List<GameSession>();

        private static List<Player> _players = new List<Player>();

        private static object _locker = new object();

        private GameSession CurrentGame { get; set; } 

        private Player CurrentPlayer { get; set; }

        public GameSessionEstablishment()
        {
            OperationContext.Current.Channel.Closed += Channel_Closed;
        }

        public void ConnectToTheGame(int gameId, string username, string password, string gamePassword = null)
        {
            if (CurrentGame == null)
            {
                var game = _games.SingleOrDefault(item => item.Description.Id == gameId);
                
                if (game != null && TryCreatePlayer(username, password))
                {
                    var result = game.ConnectPlayer(CurrentPlayer, gamePassword);
                    if (result.Result)
                    {
                        CurrentGame = game;
                        try
                        {
                            var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                            callback.SuccesfullConnectionToTheGame(CurrentGame.Description);
                        }
                        catch (Exception)
                        {
                            DisconnectFromTheGame();
                        }
                        WriteInfo();
                    }
                    else
                    {
                        RemovePlayer();
                        SendCallbackMessage(result.Message);
                    }  
                }
                else
                {
                    SendCallbackMessage("Такой игры не существует");
                }
            }
            else
            {
                SendCallbackMessage("У вас уже есть активная игра");
            }
        }
        
        public void CreateNewGame(string username, string password, string gamename, ChessDotNet.Player side, string gamePassword)
        {
            gamePassword = string.IsNullOrWhiteSpace(gamePassword) ? null : gamePassword;

            if ( CurrentGame==null & !string.IsNullOrWhiteSpace(gamename))
            {
                if(TryCreatePlayer(username, password))
                {
                    CurrentGame = new GameSession(GetNextGameId(), gamename, CurrentPlayer, side, gamePassword);
                    _games.Add(CurrentGame);
                    CurrentGame.NotifyAboutDeletingTheGame += CurrentGame_NotifyAboutDeletingTheGame;
                    try
                    {
                        var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                        callback.SuccesfullConnectionToTheGame(CurrentGame.Description);
                    }
                    catch (Exception)
                    {
                        DisconnectFromTheGame();
                    }
                    WriteInfo();
                } 
                else
                {
                    SendCallbackMessage("Не удалось создать игру");
                }
            }
            else
            {
                SendCallbackMessage("Не удалось создать игру");
            }
        }

        public void DisconnectFromTheGame()
        {
            if (CurrentGame!=null & CurrentPlayer!=null)
            {
                var result = CurrentGame.DisconnectPlayer(CurrentPlayer);
                if (result.Result) CurrentGame = null;
                RemovePlayer();
                WriteInfo();
            }
        }

        public void GetGames()
        {
            var games = new List<GameDescription>();
            foreach (var game in _games)
            {
                games.Add(game.Description);
            }
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                callback.GamesGetted(games);
            }
            catch (Exception) { }
        }

        public void Move(string originalPosition, string newPosition)
        {
            bool originalPosIsNull = string.IsNullOrWhiteSpace(originalPosition);
            bool newPosIsNull = string.IsNullOrWhiteSpace(newPosition);

            if (CurrentGame != null && CurrentPlayer != null && !originalPosIsNull && !newPosIsNull)
            {
                CurrentGame.Move(CurrentPlayer, originalPosition, newPosition);
                WriteInfo();
            }            
        }

        public void SendMessage(string message)
        {
            if(CurrentGame!=null && CurrentPlayer!=null)
            {
                CurrentGame.SendMessage(CurrentPlayer, message);
            }
        }

        public void SignupUser(string username, string password, string confirmedPassword)
        {
            if (string.IsNullOrWhiteSpace(username) | string.IsNullOrWhiteSpace(password) | string.IsNullOrWhiteSpace(confirmedPassword))
            {
                SendCallbackMessage("Некоторые поля не заполнены!");
            }
            else
            if (password != confirmedPassword)
            {
                SendCallbackMessage("Пароли не совподают!");
            }
            else
            {
                using (var context = new ChessDb.Context())
                {
                    if(context.HasUser(username))
                    {
                        SendCallbackMessage("Такой пользователь уже существует!");
                    }
                    else
                    {
                        context.CreateUser(username, password);
                        try
                        {
                            var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                            callback.SuccesfullAuthentificate(username, password, 2000);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        public void LoginUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) | string.IsNullOrWhiteSpace(password))
            {
                SendCallbackMessage("Некоторые поля не заполнены!");
            }
            else
            {
                using (var context = new ChessDb.Context())
                {
                    if (context.HasUser(username, password))
                    {
                        try
                        {
                            var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                            var rating = context.GetTotalScore(username);
                            callback.SuccesfullAuthentificate(username, password, rating);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        SendCallbackMessage("Такого пользователя не существует!");
                    }
                }
            }
        }

        public void ChangeUsername(string username, string password, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(username) | string.IsNullOrWhiteSpace(password)|string.IsNullOrWhiteSpace(newUsername))
            {
                SendCallbackMessage("Некоторые поля не заполнены!");
            }
            else
            {
                using (var context = new ChessDb.Context())
                {
                    if(context.HasUser(newUsername))
                    {
                        SendCallbackMessage("Такой пользователь уже существует!");
                    }
                    else
                    if (context.HasUser(username, password))
                    {
                        context.Users.SingleOrDefault(item => item.Username == username).Username = newUsername;
                        context.SaveChanges();
                        try
                        {
                            var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                            callback.UsernameChanged(newUsername);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        SendCallbackMessage("Неверный пароль!");
                    }
                }
            }
        }

        public void SystemRequestForGameMaintenance() { }

        private void Channel_Closed(object sender, EventArgs e)
        {
            DisconnectFromTheGame();
        }

        private void CurrentGame_NotifyAboutDeletingTheGame(GameSession game)
        {
            game.NotifyAboutDeletingTheGame -= CurrentGame_NotifyAboutDeletingTheGame;
            _games.Remove(game);
        }

        private int GetNextGameId()
        {
            if (_gameId == int.MaxValue) _gameId = 0;
            return _gameId++;
        }

        private void RemovePlayer()
        {
            if (CurrentPlayer != null)
            {
                _players.Remove(CurrentPlayer);
                CurrentPlayer = null;
            }
        }

        private bool TryCreatePlayer(string username, string password)
        {
            bool playerIsConnected = false;
            bool isExistPlayer = _players.Exists(item => item.Name == username);
            bool inputDataIsNotNull = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

            if (CurrentPlayer == null && inputDataIsNotNull && !isExistPlayer)
            {
                using (var context = new ChessDb.Context())
                {
                    if (context.HasUser(username, password))
                    {
                        var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                        CurrentPlayer = new Player(username, context.GetTotalScore(username), callback);
                        _players.Add(CurrentPlayer);
                        playerIsConnected = true;
                    }
                }
            }
            return playerIsConnected;
        }

        private void SendCallbackMessage(string message)
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IGameSessionEstablishmentCallback>();
                callback.CallbackInfo(new OperationResult(false, message));
            }
            catch (Exception) { }
        }

        static private void WriteInfo()
        {
            lock (_locker)
            {
                Console.Clear();
                for (int i = 0; i < 7; i++)
                {
                    Console.Write("================");
                }
                Console.WriteLine("\n");
                Console.WriteLine("|Players|");
                int count = 0;
                Console.WriteLine(string.Format("|{0,15}|{1,15}|{2,15}|{3,15}|{4,15}|{5,15}|", "Id:", "Username:", "Totalscore:", "RFW:", "RFL:", "RFD:"));
                foreach (var item in _players)
                {
                    count++;
                    Console.WriteLine(string.Format("|{0,15}|{1,15}|{2,15}|{3,15}|{4,15}|{5,15}|",
                        count,
                        item.Name.Length > 14 ? item.Name.Substring(0, 14) : item.Name,
                        item.TotalScore,
                        item.RatingForWin,
                        item.RatingForLose,
                        item.RatingForDraw));
                }
                for (int i = 0; i < 7; i++)
                {
                    Console.Write("................");
                }
                Console.WriteLine("\n");
                Console.WriteLine("|Games|");
                Console.WriteLine(string.Format("|{0,15}|{1,15}|{2,15}|{3,15}|{4,15}|{5,15}|{6,15}|", "Id:", "Name:", "WhitePlayer:", "BlackPlayer:", "State:", "HasPassword:", "Turn:"));

                foreach (var item in _games)
                {
                    Console.WriteLine(string.Format("|{0,15}|{1,15}|{2,15}|{3,15}|{4,15}|{5,15}|{6,15}|",
                        item.Description.Id,
                        item.Description.Name.Length > 14 ? item.Description.Name.Substring(0, 14) : item.Description.Name,
                        item.Description.WhitePlayer?.Name.Length > 14 ? item.Description.WhitePlayer?.Name.Substring(0, 14) : item.Description.WhitePlayer?.Name,
                        item.Description.BlackPlayer?.Name.Length > 14 ? item.Description.BlackPlayer?.Name.Substring(0, 14) : item.Description.BlackPlayer?.Name,
                        item.Description.StateDerscription,
                        item.Description.HasPassword,
                        item.Description.TurnDescription));
                }
                for (int i = 0; i < 7; i++)
                {
                    Console.Write("================");
                }
                Console.WriteLine("\n");
            }
        }
    }
}
