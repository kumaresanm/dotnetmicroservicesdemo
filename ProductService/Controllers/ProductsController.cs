using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private static readonly List<Product> Products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Price = 100.0m},
            new Product { Id = 2, Name = "Product2", Price = 100.0m},
            new Product { Id = 3, Name = "Product3", Price = 100.0m},
            new Product { Id = 4, Name = "Product4", Price = 100.0m},
            new Product { Id = 5, Name = "Product5", Price = 100.0m}
        };

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return Ok(Products);
        }

        [HttpPost]
        public ActionResult<IEnumerable<Product>> Post(Product product)
        {
            product.Id = Products.Max(p => p.Id) + 1;
            Products.Add(product);
            return Ok(Products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return product;
        }
    }
}

