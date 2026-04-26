using Newtonsoft.Json;

namespace CommunicationShared
{
    public class DTO<T> where T : class
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static T? FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
