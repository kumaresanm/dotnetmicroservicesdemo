using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private static readonly List<Order> Orders = new List<Order>
        {
            new Order { Id = 1, ProductId = 1, Quantity = 2 },
            new Order { Id = 2, ProductId = 2, Quantity = 1 }
        };

        public OrdersController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
        {
            return Ok(Orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound(); //handle edge cases
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://localhost:7076/api/Products/{order.ProductId}");
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response Content: {ResponseContent}", responseContent); //add proper logging

            var product = JsonSerializer.Deserialize<Product>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(new { Order = order, Product = product });
        }

        [HttpPost]
        public ActionResult<Order> Post(Order order)
        {
            order.Id = Orders.Max(o => o.Id) + 1;
            Orders.Add(order);
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
