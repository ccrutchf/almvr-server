using AlmVR.Server.Core.Providers;
using AlmVR.Server.Providers.Trello.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlmVR.Server.Providers.Trello
{
    public class TrelloWebHookProvider : TrelloProviderBase
    {
        private readonly Regex firstUpperCaseRegex;

        private string tokenURL;

        public TrelloWebHookProvider(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "webhooks")
        {
            firstUpperCaseRegex = new Regex("(^[A-Z]*)", RegexOptions.Compiled);
        }

        protected override Task OnInitializedAsync()
        {
            tokenURL = $"{TRELLO_URL}/tokens/{Token}";

            return base.OnInitializedAsync();
        }

        public async Task DeleteWebHookAsync(TrelloWebHook webHook)
        {
            await InitializeAsync();

            var url = $"{TRELLO_URL}/webhooks/{webHook.ID}?{KeyAndToken}";

            using (var response = await HttpClient.DeleteAsync(url))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<IEnumerable<TrelloWebHook>> GetWebHooksAsync()
        {
            await InitializeAsync();

            var url = $"{tokenURL}/webhooks?{KeyAndToken}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();
                var webHooks = JsonConvert.DeserializeObject<IEnumerable<TrelloWebHook>>(content);

                return webHooks;
            }
        }

        public async Task PurgeWebHooksAsync()
        {
            var tasks = (from wh in await GetWebHooksAsync()
                         select DeleteWebHookAsync(wh)).ToArray();

            await Task.WhenAll(tasks);
        }

        public async Task SetAsync(TrelloWebHook webHook)
        {
            await InitializeAsync();

            var existingWebHooks = await GetWebHooksAsync();

            var match = (from wh in existingWebHooks
                         where wh.Match(webHook)
                         select wh).SingleOrDefault();

            if (match != null)
            {
                if (match?.Equals(webHook) ?? false)
                {
                    return;
                }

                var url = $"{BaseUrl}/{match.ID}?{KeyAndToken}&{BuildUrl(match)}";

                using (var content = new StringContent(string.Empty))
                using (var response = await HttpClient.PutAsync(url, content))
                {
                    var result = await content.ReadAsStringAsync();

                    response.EnsureSuccessStatusCode();
                }
            }
            else
            {
                var url = $"{BaseUrl}?{KeyAndToken}&{BuildUrl(webHook)}";

                using (var content = new StringContent(string.Empty))
                using (var response = await HttpClient.PostAsync(url, content))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private string BuildUrl(TrelloWebHook webHook)
        {
            var properties = typeof(TrelloWebHook).GetProperties();

            var stringBuilder = new StringBuilder();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(webHook);
                if (value == null ||
                    (value is string &&
                    string.IsNullOrWhiteSpace((string)value)))
                    continue;

                string valueString = null;

                if (value is string)
                    valueString = (string)value;
                else
                    valueString = JsonConvert.SerializeObject(value);

                stringBuilder.Append($"{ConvertToCamelCase(prop.Name)}={valueString}&");
            }

            var result = stringBuilder.ToString();

            return result.Substring(0, result.Length - 1);
        }

        private string ConvertToCamelCase(string input)
        {
            var matches = firstUpperCaseRegex.Matches(input);

            var upper = matches
                        .Cast<Match>()
                        .SingleOrDefault()?
                        .Value;

            var lower = upper?.ToLowerInvariant();

            return $"{lower}{input.Substring(lower.Length)}";
        }
    }
}
