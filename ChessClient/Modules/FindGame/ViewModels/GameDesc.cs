using ChessClient.GameSessionEstablishmentServiceRef;

namespace ChessClient.Modules.FindGame.ViewModels
{
    public class GameDesc
    {
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                switch (_state)
                {
                    case "Waiting": StateFieldColor = "Blue"; break;
                    case "Playing": StateFieldColor = "Green"; break;
                    default: StateFieldColor = "Red"; break;
                }
            }
        }

        public bool HasPassword
        {
            get { return _hasPassword; }
            set
            {
                _hasPassword = value;
                if (value) HasPasswordImagePath = "/Resources/Images/lock.png";
                else HasPasswordImagePath = "/Resources/Images/open.png";
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string HasPasswordImagePath { get; set; }

        public string WhiteUsername { get; set; }

        public int? WhiteRating { get; set; }

        public int? WhiteRatingForLose { get; set; }

        public int? WhiteRatingForWin { get; set; }

        public int? WhiteRatingForDraw { get; set; }

        public string BlackUsername { get; set; }

        public int? BlackRating { get; set; }

        public int? BlackRatingForLose { get; set; }

        public int? BlackRatingForWin { get; set; }

        public int? BlackRatingForDraw { get; set; }

        public string StateFieldColor { get; set; }

        public string TurnState { get; set; }

        private string _state;

        private bool _hasPassword;

        public GameDesc(int id, string name, string state, bool hasPassword, string whiteUsername=null, int? whiteRating=null, string blackUsername=null, int? blackRating=null )
        {
            Id = id;
            Name = name;
            State = state;
            HasPassword = hasPassword;
            WhiteUsername = whiteUsername;
            WhiteRating = whiteRating;
            BlackUsername = blackUsername;
            BlackRating = blackRating;
        }
        public GameDesc(GameDescription gameDescription)
        {
            Id = gameDescription.Id;
            Name = gameDescription.Name;
            State = gameDescription.StateDerscription;
            HasPassword = gameDescription.HasPassword;
            TurnState = gameDescription.TurnDescription;

            WhiteUsername = gameDescription.WhitePlayer?.Name;
            WhiteRating = gameDescription.WhitePlayer?.TotalScore;
            WhiteRatingForWin = gameDescription.WhitePlayer?.RatingForWin;
            WhiteRatingForLose = gameDescription.WhitePlayer?.RatingForLose;
            WhiteRatingForDraw = gameDescription.WhitePlayer?.RatingForDraw;

            BlackUsername = gameDescription.BlackPlayer?.Name;
            BlackRating = gameDescription.BlackPlayer?.TotalScore;
            BlackRatingForWin = gameDescription.BlackPlayer?.RatingForWin;
            BlackRatingForLose = gameDescription.BlackPlayer?.RatingForLose;
            BlackRatingForDraw = gameDescription.BlackPlayer?.RatingForDraw;
        }
    }
}


