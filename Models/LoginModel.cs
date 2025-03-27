namespace PrimeraWebAPI.Models
{
    public class LoginModel {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginModel(string username, string password) {
            Username = username;
            Password = password;
        }

        public LoginModel(){
            Username = "";
            Password = "";
        }
    }
}