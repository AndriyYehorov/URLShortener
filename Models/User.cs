using URLShortener.Enums;

namespace URLShortener.Models
{
    public class User
    {
        public User(string login, string password)
        {
            Login = login;
            Password = password;
            RoleId = (int)RolesEnum.User;
        }

        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int RoleId { get; set; }

        public Role? Role { get; set; }
    }
}
