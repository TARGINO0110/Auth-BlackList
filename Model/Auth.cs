
using System.Text.Json.Serialization;

namespace Auth_BlackList.Model
{
    public class Auth
    {
        public string User { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public bool ValidUser { get => ValueUsers(User, Password); }
        
        private static bool ValueUsers(string user, string password)
        {
            List<Auth> users =
            [
                new Auth { User = "USER1", Password = "PASS1" },
                new Auth { User = "USER2", Password = "PASS2" }
            ];

            return users.Any(u => u.User == user && u.Password == password);
        }
    }
}
