using System.Collections.Generic;

namespace Tips.Security
{
    public class ApiKeyConfiguration
    {
        public string ApiHeader { get; set; }
        public IEnumerable<ApiKey> ApiKeys { get; set; }
    }
}