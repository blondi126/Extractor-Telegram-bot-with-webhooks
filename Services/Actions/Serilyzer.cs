using System.Xml.Serialization;
using UnzipBot.Models;

namespace UnzipBot.Services.Actions
{
    public static class Serilyzer
    {
        public static async Task SerializeDictionary()
        {
            var xmlSerializer = new XmlSerializer(typeof(SerializableDictionary<long, AuthInfo>));

            await using var fs = new FileStream("AuthsData.xml", FileMode.Create);
            xmlSerializer.Serialize(fs, UserInfo.AuthsDictionary);
        }
        public static async Task<SerializableDictionary<long, AuthInfo>> DeserializeDictionary()
        {
            var xmlSerializer = new XmlSerializer(typeof(SerializableDictionary<long, AuthInfo>));

            await using var fs = new FileStream("AuthsData.xml", FileMode.Open);
            return (xmlSerializer.Deserialize(fs) as SerializableDictionary<long, AuthInfo>)!;
        }



    }
}
