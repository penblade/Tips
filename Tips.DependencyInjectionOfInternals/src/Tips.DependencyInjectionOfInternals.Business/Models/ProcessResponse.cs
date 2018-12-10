using System.Collections.Generic;

namespace Tips.DependencyInjectionOfInternals.Business.Models
{
    public class ProcessResponse
    {
        public ProcessResponse()
        {
            Messages = new List<string>();
        }
        // Leave the setter public for deserialization.
        public List<string> Messages { get; set; }
    }
}
