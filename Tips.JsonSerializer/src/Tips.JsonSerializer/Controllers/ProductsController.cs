using Microsoft.AspNetCore.Mvc;
using Tips.JsonSerializer.Models;

namespace Tips.JsonSerializer.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [Route("api/products")]
        [HttpGet]
        public ActionResult Get(ProductRequest request)
        {
            // Return the request formatted nicely to quickly see
            // if the request deserialized and serialized correctly.
            return Ok(request);
        }
    }
}
