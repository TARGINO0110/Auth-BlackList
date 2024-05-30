using Auth_BlackList.Model;
using System.Text.Json;

namespace Auth_BlackList.Utils
{
    public class DeserializeJson
    {
        public SecurityIpModel DeserializeObjectJson(string json)
        {
            return JsonSerializer.Deserialize<SecurityIpModel>(json);
        }

        public string SerializeObjectJson(SecurityIpModel securityIpModel)
        {
            return JsonSerializer.Serialize<SecurityIpModel>(securityIpModel);
        }
    }
}
