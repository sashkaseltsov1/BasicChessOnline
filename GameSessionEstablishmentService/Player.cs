using System;
using System.Runtime.Serialization;

namespace GameSessionEstablishmentService
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int TotalScore { get; set; }

        public IGameSessionEstablishmentCallback Callback { get; }

        [DataMember]
        public int RatingForWin { get; set; }

        [DataMember]
        public int RatingForLose { get; set; }

        [DataMember]
        public int RatingForDraw { get; set; }

        public Player(string name, int totalScore, IGameSessionEstablishmentCallback callback)
        {
            if (string.IsNullOrWhiteSpace(name) | callback == null) throw new ArgumentNullException();
            if (TotalScore < 0) throw new ArgumentOutOfRangeException();

            Name = name;
            TotalScore = totalScore;
            Callback = callback;
            SetDefaultRates();
        }

        public void CalculateRates(Player player)
        {
            RatingForWin = CalculateRates(TotalScore, player.TotalScore, 1);

            RatingForLose = CalculateRates(TotalScore, player.TotalScore, 0);

            RatingForDraw = CalculateRates(TotalScore, player.TotalScore, 0.5);
        }

        public void UpdateTotalScore(int value)
        {
            RecalculateTotalScore(value);

            using (var context = new ChessDb.Context())
            {
                context.PushTotalScoreToTheDatabase(Name, TotalScore);
            }

            SetDefaultRates();
        }

        private void RecalculateTotalScore(int value)
        {
            if (TotalScore + value < 0) TotalScore = 0; else TotalScore += value;
        }

        private int CalculateRates(double Rating1, double Rating2, double result)
        {
            double dif = Rating2 - Rating1;
            if (dif > 400) dif = 400;
            double n = dif / 400;
            double Ea = 1 / (1 + Math.Pow(10, n));
            return Convert.ToInt32(40 * (result - Ea));
        }

        private void SetDefaultRates()
        {
            RatingForWin = 0;
            RatingForLose = 0;
            RatingForDraw = 0;
        }
    }
}
