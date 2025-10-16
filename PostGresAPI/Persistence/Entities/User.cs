namespace PostGresAPI.Models
{
    public class User
    {
        private User() { } 

        public User(string userName, string email, string phone = "")
        {
            UserName = userName;
            Email = email ?? "";
            Phone = phone ?? "";
        }

        public int Id { get; private set; }
        public string UserName { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string Phone { get; private set; } = "";


        internal void Apply(string userName, string email, string phone = "")
        {
            UserName = userName;
            Email = email ?? "";
            Phone = phone ?? "";
        }
    }
}
