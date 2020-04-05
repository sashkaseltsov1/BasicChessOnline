using System.Data.Entity;
using System.Linq;

namespace ChessDb
{
    public class Context: DbContext
    {
        public DbSet<User> Users { get; set; }

        public Context() : base("chessdb") { }

        public void PushTotalScoreToTheDatabase(string username, int totalScore)
        {
            var user = Users.SingleOrDefault(item => item.Username == username);
            if (user != null)
            {
                user.TotalScore = totalScore;
                SaveChanges();
            }
        }

        public bool HasUser(string username, string password)
        {
            return Users.Where(item => item.Username == username && item.Password == password).Count() > 0;
        }

        public bool HasUser(string username)
        {
            return Users.Where(item => item.Username == username).Count() > 0;
        }

        public void CreateUser(string username, string password, int rating=2000)
        {
            Users.Add(new User() { Username = username, Password = password, TotalScore = rating });
            SaveChanges();
        }

        public int GetTotalScore(string username)
        {
            var user = Users.SingleOrDefault(item => item.Username == username);
            if (user != null) return user.TotalScore; else return 0;
        }

    }
}
