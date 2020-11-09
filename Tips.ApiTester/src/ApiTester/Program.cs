using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTester
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();
        static async Task Main(string[] args)
        {
            var processor = new TodoItemProcessor(Client);
            await processor.Process();
            Console.WriteLine("Hello World!");
        }
    }
}
