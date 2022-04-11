using UnzipBot.Services.Actions;

namespace UnzipBot.Models
{
    public static class UserInfo
    {
        internal static SerializableDictionary<long, AuthInfo> AuthsDictionary;

        static UserInfo()
        {
            AuthsDictionary = Serilyzer.DeserializeDictionary().Result;
        }
    }
}
