// PostGresAPI/Models/User.cs
namespace PostGresAPI.Models
{
    public class User
    {
        private User() { } // EF

        public User(string userName, string email, string phone = "")
        {
            UserName = userName;
            Email = email ?? "";
            Phone = phone ?? "";
        }

        public int Id { get; private set; }

        public string UserName { get; internal set; } = "";
        public string Email { get; internal set; } = "";
        public string Phone { get; internal set; } = "";
    }
}
