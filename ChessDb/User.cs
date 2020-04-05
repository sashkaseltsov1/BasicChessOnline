
namespace ChessDb
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int TotalScore { get; set; } = 1300;
    }
}
