using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace BillTracker.Tests.Helpers
{
    public static class ContentHelper
    {
        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");

        public static T ReadFromString<T>(string obj) => JsonConvert.DeserializeObject<T>(obj);
    }
}
