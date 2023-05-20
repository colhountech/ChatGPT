using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatGPT
{
    public class OpenAiClient
    {
        private readonly HttpClient _httpClient;

        public OpenAiClient(string apiKey)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<Response> SendRequest(string prompt, string model = "text-davinci-002", int maxTokens = 128, bool echo = false)
        {
            string url = GetApiUrl(model);
            object requestParams = GetRequestParams(model, prompt, maxTokens, echo);

            var requestJson = JsonSerializer.Serialize(requestParams);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, requestContent);

            if (await response.Content.ReadAsStringAsync() is string responseJson)
            {
                return JsonSerializer.Deserialize<Response>(responseJson);
            }

            return null;
        }

        private string GetApiUrl(string model)
        {
            switch (model)
            {
                case "gpt-3.5-turbo":
                case "gpt-4":
                    return "https://api.openai.com/v1/chat/completions";
                default:
                    // text-davinci-002
                    return "https://api.openai.com/v1/completions"; 
            }
        }

        private object GetRequestParams(string model, string prompt, int maxTokens, bool echo)
        {
            switch (model)
            {
                case "gpt-3.5-turbo":
                case "gpt-4":
                    return new
                    {
                        model = model,
                        messages = new[]
                        {
                            new { role = "user", content = prompt }
                        }
                    };
                default: // text-davinci-002
                    return new
                    {
                        prompt = prompt,
                        model = model,
                        max_tokens = maxTokens,
                        echo = echo,
                    };
            }
        }

    }
}
