namespace UnzipBot.Models
{
    public class AuthInfo
    {
        public AuthInfo()
        {

        }
        public AuthInfo(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email = null!;
        public string Password = null!;


    }
}
