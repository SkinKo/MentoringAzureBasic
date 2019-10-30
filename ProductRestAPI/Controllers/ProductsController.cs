using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductRestAPI.Models;
using ProductRestAPI.Repository;

namespace ProductRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        // GET api/products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return _productsRepository.Get().ToArray();
        }

        // GET api/products/5
        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            if (id <= 0)
                throw new KeyNotFoundException();

            return _productsRepository.Get(id);
        }

        // POST api/products
        [HttpPost]
        public void Post([FromBody] Product value)
        {
            _productsRepository.Add(value);
        }

        // PUT api/products/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Product value)
        {
            _productsRepository.Update(id, value);
        }

        // DELETE api/products/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if(id <= 0)
                throw new KeyNotFoundException();

            _productsRepository.Delete(id);
        }
    }
}
