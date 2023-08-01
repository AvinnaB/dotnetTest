using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace APIControllers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private IProductRepository productRepository;

        public HomeController(IProductRepository repo)
        {
            productRepository = repo;
        }
        public string Index()
        {
            return "API Running...";
        }

        [HttpGet]
        public IEnumerable<Product> GetActiveProducts()
        {
            return productRepository.ActiveProducts().ToList();
        }

        [HttpGet]
        public IActionResult GetString()
        {
            var activeProducts = "All is well";

            return Ok(activeProducts);
        }

        [HttpGet("search")]
        public IEnumerable<Product> SearchProducts(string productName, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            return productRepository.SearchProduct(productName, minPrice, maxPrice, startDate, endDate).ToList();
        }

        [HttpPost]
        public IActionResult CreateProduct(string productName, decimal price)
        {
            if (price > 10000)
                return BadRequest("Product price exceeds the limit.");

            // Check if the product needs approval
            if (price > 5000)
            {
                bool rowsAffected = productRepository.CreateApprovalProduct(productName, price);
                if (rowsAffected == true)
                {
                    return Ok("Product added to the approval queue.");
                }
                else
                {
                    return Ok("Error is occured.");
                }
            }
            else
            {
                bool rowsAffected = productRepository.CreateProduct(productName, price);
                if (rowsAffected == true)
                {
                    return Ok("Product created successfully.");
                }
                else
                {
                    return Ok("Error is occured.");
                }

            }

        }

        [HttpPut]
        public IActionResult UpdateProduct(int productId, string productName, decimal price)
        {
            var products = productRepository.ActiveProductsEdit(productId);

            if (products == null)
                return NotFound();

            if (price > 10000)
                return BadRequest("Product price exceeds the limit.");

            // Check if the product needs approval
            bool requiresApproval = (price > 5000);

            if (requiresApproval)
            {
                // Add the product to the approval queue
                bool rowsAffected = productRepository.UpdateApprovalProduct(productId, productName, price);
                if (rowsAffected == true)
                {
                    return Ok("Product added to the approval queue.");
                }
                else
                {
                    return Ok("Error is occured.");
                }
            }
            else
            {
                bool rowsAffected = productRepository.UpdateProduct(productId, productName, price);
                if (rowsAffected == true)
                {
                    return Ok("Product update successfully.");
                }
                else
                {
                    return Ok("Error is occured.");
                }
            }

        }

        [HttpDelete]
        public IActionResult DeleteProduct(int productId)
        {

            // Add the product to the approval queue
            bool rowsAffected = productRepository.DeleteProduct(productId);
            if (rowsAffected == true)
            {
                return Ok("Product deletion request added to the approval queue.");

            }
            else
            {
                return Ok("Error is occured.");
            }
        }
    }
}