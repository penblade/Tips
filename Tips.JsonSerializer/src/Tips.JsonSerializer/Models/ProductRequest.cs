using System.Collections.Generic;
using Tips.JsonSerializer.Models.Nested;

namespace Tips.JsonSerializer.Models
{
    public class ProductRequest
    {
        public Product1 A { get; set; }

        public Product2 B { get; set; }
        public Product C { get; set; }
        public Product D { get; set; }
        public List<Product1> AList { get; set; }
        public List<Product2> BList { get; set; }
        public List<Product> CList { get; set; }
        public List<Product> DList { get; set; }
    }
}
