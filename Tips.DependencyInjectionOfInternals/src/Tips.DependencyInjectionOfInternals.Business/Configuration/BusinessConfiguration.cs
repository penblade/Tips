using System.Collections.Generic;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    public class BusinessConfiguration
    {
        public string ConnectionString { get; set; }
        public string DocumentPath { get; set; }
        public List<string> IocFiles { get; set; }
    }
}
