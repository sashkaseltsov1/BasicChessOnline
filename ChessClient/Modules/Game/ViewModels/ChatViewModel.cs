using ChessClient.Client;
using ChessClient.Modules.EventAgregator;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace ChessClient.Modules.Game.ViewModels
{
    class ChatViewModel:BindableBase
    {
        public DelegateCommand SendMessageCommand { get; set; }

        public string Messages
        {
            get { return _messages; }
            set { SetProperty(ref _messages, value); }
        }

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private readonly IEventAggregator _ea;

        private string _message;

        private string _messages;

        public ChatViewModel(IEventAggregator ea)
        {
            _ea = ea;

            SendMessageCommand = new DelegateCommand(SendMessage);

            _ea.GetEvent<GameDescriptionSentEvent>().Subscribe((desc)=> { Message = null;Messages = null; });
            
            TcpChessClient.getInstance().MessageRecieved += (username, message) => 
            {
                string newMessage = username + " : " + message + "\n";
                Messages += newMessage;
            };
        }

        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                TcpChessClient.getInstance().SendMessage(Message);
                string newMessage = "Вы : " + Message + "\n";
                Messages += newMessage;
                Message = null;
            }
        }
    }
}
