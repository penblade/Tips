using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Tips.JsonSerializer.Configuration;
using Tips.JsonSerializer.Models;
using Tips.JsonSerializer.Models.Nested;

namespace Tips.JsonSerializer.Tests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void CreateSerializedDataForPostmanTest()
        {
            var request = new ProductRequest
            {
                A = new Product1
                {
                    Id = 1,
                    UniqueProperty1 = "value 1"
                },
                B = new Product2
                {
                    Id = 2,
                    UniqueProperty2 = "value 2"
                },
                C = new Product1
                {
                    Id = 3,
                    UniqueProperty1 = "value 3"
                },
                D = new Product2
                {
                    Id = 4,
                    UniqueProperty2 = "value 4"
                },
                AList = new List<Product1>
                {
                    new Product1
                    {
                        Id = 1001,
                        UniqueProperty1 = "value 1001"
                    },
                    new Product1
                    {
                        Id = 1002,
                        UniqueProperty1 = "value 1002"
                    },
                    new Product1
                    {
                        Id = 1003,
                        UniqueProperty1 = "value 1003"
                    }
                },
                BList = new List<Product2>
                {
                    new Product2
                    {
                        Id = 2001,
                        UniqueProperty2 = "value 2001"
                    },
                    new Product2
                    {
                        Id = 2002,
                        UniqueProperty2 = "value 2002"
                    },
                    new Product2
                    {
                        Id = 2003,
                        UniqueProperty2 = "value 2003"
                    }
                },
                CList = new List<Product>
                {
                    new Product1
                    {
                        Id = 3001,
                        UniqueProperty1 = "value 3001"
                    },
                    new Product2
                    {
                        Id = 3002,
                        UniqueProperty2 = "value 3002"
                    },
                    new Product1
                    {
                        Id = 3003,
                        UniqueProperty1 = "value 3003"
                    }
                },
                DList = new List<Product>
                {
                    new Product2
                    {
                        Id = 4001,
                        UniqueProperty2 = "value 4001"
                    },
                    new Product1
                    {
                        Id = 4002,
                        UniqueProperty1 = "value 4002"
                    },
                    new Product2
                    {
                        Id = 4003,
                        UniqueProperty2 = "value 4003"
                    }
                }
            };

            var namespaceToTypes = typeof(Product).Namespace;
            var settings = new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                SerializationBinder = new CustomJsonSerializationBinder(namespaceToTypes)
            };

            var serialized = JsonConvert.SerializeObject(request, settings);

            // Create a new request in Postman
            // using the url for this app's localhost.

            // Copy the text in the serialized variable.

            // Paste into Postman Body.
            // Change the ContentType to JSON (application/json)

            // Click Send
        }
    }
}
