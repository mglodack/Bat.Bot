using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bat.Bot
{
    public class Luis
    {
        internal static string Id => Environment.GetEnvironmentVariable("BOT_ID", EnvironmentVariableTarget.User);

        internal static string SubscriptionKey => Environment.GetEnvironmentVariable("BOT_SUBSCRIPTION_KEY", EnvironmentVariableTarget.User);

        internal static string BaseUrl => "https://api.projectoxford.ai/luis/v1/application";

        public static async Task<Rootobject> Ask(string utterance)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(_BuildFullUri(utterance)).ConfigureAwait(false))
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<Rootobject>(json);
                }
            }
        }

        static string _BuildFullUri(string utterance)
        {
            return $"{BaseUrl}{_GetQueryString(utterance)}";
        }

        static string _AppendQueryParameters(string seed, KeyValuePair<string, string> kvp)
        {
            return $"{seed}&{kvp.Key}={kvp.Value}";
        }

        static string _SeedQuery(KeyValuePair<string, string> kvp)
        {
            return $"?{kvp.Key}={kvp.Value}";
        }

        static Dictionary<string, string> _BaseParams()
        {
            return new Dictionary<string, string>()
            {
                { "id", Id },
                { "subscription-key", SubscriptionKey },
            };
        }

        static Dictionary<string, string> _QueryParams(string utterance)
        {
            var parameters = _BaseParams();
            parameters.Add("q", utterance);
            return parameters;
        }

        static string _GetQueryString(string utterance)
        {
            var queryParams = _QueryParams(HttpUtility.UrlEncode(utterance));
            return queryParams
                .Skip(1)
                .Aggregate(_SeedQuery(queryParams.FirstOrDefault()), _AppendQueryParameters);
        }
    }

    public class Rootobject
    {
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
        public object actions { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }
}