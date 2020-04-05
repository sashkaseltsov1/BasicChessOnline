using ChessClient.GameSessionEstablishmentServiceRef;
using System;
using System.ServiceModel;

namespace ChessClient.Client
{
    public class TcpChessClient
    {
        public delegate void CallbackInfoHandler(OperationResult result);

        public delegate void GameDescriptionResultHandler(GameDescription game);

        public delegate void GamesResultHandler(GameDescription[] game);

        public delegate void DoubleParamStringHandler(string str1, string str2);

        public delegate void SimpleHandler();

        public event CallbackInfoHandler CallbackInfoRecieved;

        public event GameDescriptionResultHandler ConnectedToTheGame;

        public event GameDescriptionResultHandler GameStateChanged;

        public event SimpleHandler Authentificated;

        public event SimpleHandler CommandExecutionStarted;

        public event SimpleHandler CommandExecutionStoped;

        public event GamesResultHandler GamesRecieved;

        public event DoubleParamStringHandler MessageRecieved;

        public event DoubleParamStringHandler PlayerMoved;

        private static TcpChessClient _instance;

        private System.Timers.Timer _timerForGameEstablishment;

        public event SimpleHandler UsernameChanged;

        private GameSessionEstablishmentClient client;

        public string Username { get; set; }

        public int? Rating { get; set; }

        public string Password { get; set; }

        private TcpChessClient()
        {
            InitializeGameSessionClient();
        }

        public static TcpChessClient getInstance()
        {
            if (_instance == null )
            {
                _instance = new TcpChessClient();
            }
            else
            {
                if(_instance.client.State==CommunicationState.Closed | _instance.client.State==CommunicationState.Faulted)
                {
                    _instance.InitializeGameSessionClient();
                }
            }
            return _instance;
        }

        public async void LoginUser(string username, string password)
        {
            CommandExecutionStarted();
            try
            {
                await _instance.client.LoginUserAsync(username, password);
            }
            catch (Exception ex)
            {
                CommandExecutionStoped();
                ShowInfo(ex);
            }
        }

        public async void SignupUser(string username, string password, string confirmedPassword)
        {
            CommandExecutionStarted();
            try
            {
                await _instance.client.SignupUserAsync(username, password, confirmedPassword);
            }
            catch (Exception ex)
            {
                CommandExecutionStoped();
                ShowInfo(ex);
            }
        }

        public async void Move(string originalPosition, string newPosition)
        {
            try
            {
                await _instance.client.MoveAsync(originalPosition, newPosition);
            }
            catch (Exception ex)
            {

                ShowInfo(ex);
            }
        }

        public async void CreateNewGame(string gameName, ChessDotNet.Player side, string gamePassword)
        {
            CommandExecutionStarted();
            try
            {
                await _instance.client.CreateNewGameAsync(Username, Password, gameName, side, gamePassword);
            }
            catch (Exception ex)
            {
                CommandExecutionStoped();
                ShowInfo(ex);
            }
        }

        public async void ConnectToTheGame(int gameId, string gamePassword)
        {
            CommandExecutionStarted();
            try
            {
                await _instance.client.ConnectToTheGameAsync(gameId, _instance.Username, _instance.Password, gamePassword);
            }
            catch (Exception ex)
            {
                CommandExecutionStoped();
                ShowInfo(ex);
            }
        }

        public async void SendMessage(string message)
        {
            try
            {
                await _instance.client.SendMessageAsync(message);
            }
            catch (Exception){ }
        }

        public async void ChangeUsername(string newUsername, string password)
        {
            CommandExecutionStarted();
            try
            {
                await _instance.client.ChangeUsernameAsync(_instance.Username, password, newUsername);
            }
            catch (Exception ex)
            {
                CommandExecutionStoped();
                ShowInfo(ex);
            }
        }

        public async void DisconnectFromTheGame()
        {
            RemoveTimer();
            try
            {
                await _instance.client.DisconnectFromTheGameAsync();
            }
            catch (Exception) { }
        }

        public async void GetGames()
        {
            CommandExecutionStarted();
            try
            {
                await _instance.client.GetGamesAsync();
            }
            catch (Exception ex)
            {
                CommandExecutionStoped();
                ShowInfo(ex);
            }
        }

        public void ShowInfo(string message)
        {
            var result = new OperationResult();
            result.Message = message;
            CallbackInfoRecieved(result);
        }

        private void SetTimer()
        {
            _timerForGameEstablishment = new System.Timers.Timer(840000);
            _timerForGameEstablishment.Elapsed += (source, e) => 
            {
                try
                {
                    _instance.client.SystemRequestForGameMaintenanceAsync();
                }
                catch (Exception){ }
            };
            _timerForGameEstablishment.AutoReset = true;
            _timerForGameEstablishment.Enabled = true;
            _timerForGameEstablishment.Start();
        }

        private void RemoveTimer()
        {
            _timerForGameEstablishment.Stop();
            _timerForGameEstablishment.Dispose();
        }

        private void InitializeGameSessionClient()
        {
            var callback = new Callback();
            callback.CallbackInfoRecieved += (result) => { CallbackInfoRecieved(result);CommandExecutionStoped(); };
            callback.ConnectedToTheGame += (game) => 
            {
                SetTimer();
                ConnectedToTheGame(game); CommandExecutionStoped();
                
            };
            callback.GameStateChanged += (game) => 
            {
                if(game.StateDerscription=="Playing")
                {
                    ShowInfo("Игра началась!");
                }
                if (game.StateDerscription == "Ended")
                {
                    ShowInfo(game.Result);
                }
                GameStateChanged(game);
            };
            callback.GamesRecieved += (game) => { GamesRecieved(game); CommandExecutionStoped(); };
            callback.MessageRecieved += (name, msg) => { MessageRecieved(name, msg); };
            callback.PlayerMoved += (move1, move2) => { PlayerMoved(move1, move2); };
            callback.UsernameFieldChanged += (username) => { Username = username;UsernameChanged(); CommandExecutionStoped(); };
            callback.Authentificated += (username, password, rating) => 
            {
                Username = username;
                Password = password;
                Rating = rating;
                CommandExecutionStoped();
                Authentificated();
            };

            InstanceContext instanceContext = new InstanceContext(callback);
            client = new GameSessionEstablishmentClient(instanceContext);
        }

        private void ShowInfo(Exception ex)
        {
            var result = new OperationResult();
            result.Message = ex.Message;
            CallbackInfoRecieved(result);
        }
    }
}
