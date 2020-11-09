using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiTester
{
    public class TodoItemProcessor
    {
        private readonly HttpClient _httpClient;

        public TodoItemProcessor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TodoItem> Process()
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            var activity = new Activity("TodoItems").Start();
            var traceId = Activity.Current?.Id;
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", "77D4AA72-16F8-47D4-B237-DE285A271BF8");
                var apiResponse = await _httpClient.GetAsync("https://localhost:44305/api/TodoItems/1");
                var todoItemJson = await apiResponse.Content.ReadAsStringAsync();
                var todoItem = JsonSerializer.Deserialize<TodoItem>(todoItemJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return todoItem;
            }
            finally
            {
                activity.Stop();
            }
        }
    }
}
