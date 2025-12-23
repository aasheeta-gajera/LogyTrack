// ====================================================================
// ProductsController.cs - Complete Product API Endpoints
// ====================================================================
// Location: LogyTrackAPI/Controllers/ProductsController.cs

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogyTrackAPI.Models;
using LogyTrackAPI.Infrastructure.Data.Products;

namespace LogyTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // ===== GET ALL PRODUCTS WITH VEHICLE AND DRIVER DETAILS =====
        /// <summary>
        /// Get all products with vehicle and driver information
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                if (products == null || !products.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "No products found",
                        data = new List<Product>(),
                        count = 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Products retrieved successfully",
                    data = products,
                    count = products.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET PRODUCT BY ID =====
        /// <summary>
        /// Get a specific product by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid product ID" });

                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return NotFound(new { success = false, message = "Product not found" });

                return Ok(new
                {
                    success = true,
                    message = "Product retrieved successfully",
                    data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET PRODUCTS BY STATUS =====
        /// <summary>
        /// Get products filtered by status (Unassigned, Assigned, InTransit, Delivered, Returned)
        /// </summary>
        [HttpGet("status/{status}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return BadRequest(new { success = false, message = "Status is required" });

                var validStatuses = new[] { "Unassigned", "Assigned", "InTransit", "Delivered", "Returned" };
                if (!validStatuses.Contains(status))
                    return BadRequest(new { success = false, message = "Invalid status. Must be one of: Unassigned, Assigned, InTransit, Delivered, Returned" });

                var products = await _productRepository.GetByStatusAsync(status);
                return Ok(new
                {
                    success = true,
                    message = $"Products with status '{status}' retrieved successfully",
                    data = products,
                    count = products.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET PRODUCTS BY VEHICLE =====
        /// <summary>
        /// Get all products assigned to a specific vehicle
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByVehicle(int vehicleId)
        {
            try
            {
                if (vehicleId <= 0)
                    return BadRequest(new { success = false, message = "Invalid vehicle ID" });

                var products = await _productRepository.GetByVehicleAsync(vehicleId);
                return Ok(new
                {
                    success = true,
                    message = "Products retrieved for vehicle successfully",
                    data = products,
                    count = products.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET PRODUCTS BY DRIVER =====
        /// <summary>
        /// Get all products assigned to a specific driver
        /// </summary>
        [HttpGet("driver/{driverId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByDriver(int driverId)
        {
            try
            {
                if (driverId <= 0)
                    return BadRequest(new { success = false, message = "Invalid driver ID" });

                var products = await _productRepository.GetByDriverAsync(driverId);
                return Ok(new
                {
                    success = true,
                    message = "Products retrieved for driver successfully",
                    data = products,
                    count = products.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET UNASSIGNED PRODUCTS =====
        /// <summary>
        /// Get all products that are not assigned to any vehicle or driver
        /// </summary>
        [HttpGet("unassigned/list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUnassigned()
        {
            try
            {
                var products = await _productRepository.GetUnassignedAsync();
                return Ok(new
                {
                    success = true,
                    message = "Unassigned products retrieved successfully",
                    data = products,
                    count = products.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== CREATE PRODUCT =====
        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Product request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.ProductName))
                    return BadRequest(new { success = false, message = "Product name is required" });

                if (string.IsNullOrWhiteSpace(request.SKU))
                    return BadRequest(new { success = false, message = "SKU is required" });

                if (request.Quantity <= 0)
                    return BadRequest(new { success = false, message = "Quantity must be greater than 0" });

                if (request.UnitPrice <= 0)
                    return BadRequest(new { success = false, message = "Unit price must be greater than 0" });

                var newProduct = new Product
                {
                    ProductName = request.ProductName.Trim(),
                    SKU = request.SKU.Trim().ToUpper(),
                    Description = request.Description?.Trim() ?? "",
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    VehicleId = request.VehicleId,
                    DriverId = request.DriverId,
                    Status = "Unassigned",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                var id = await _productRepository.CreateAsync(newProduct);
                newProduct.ProductId = id;

                return CreatedAtAction(nameof(GetById), new { id }, new
                {
                    success = true,
                    message = "Product created successfully",
                    data = newProduct
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== UPDATE PRODUCT =====
        /// <summary>
        /// Update product information (name, SKU, quantity, price, status)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Product request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid product ID" });

                if (request == null || string.IsNullOrWhiteSpace(request.ProductName))
                    return BadRequest(new { success = false, message = "Product name is required" });

                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                    return NotFound(new { success = false, message = "Product not found" });

                var validStatuses = new[] { "Unassigned", "Assigned", "InTransit", "Delivered", "Returned" };
                if (!validStatuses.Contains(request.Status))
                    return BadRequest(new { success = false, message = "Invalid status" });

                var updatedProduct = new Product
                {
                    ProductId = id,
                    ProductName = request.ProductName.Trim(),
                    SKU = request.SKU.Trim().ToUpper(),
                    Description = request.Description?.Trim() ?? "",
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    Status = request.Status,
                    CreatedDate = existingProduct.CreatedDate,
                    UpdatedDate = DateTime.Now
                };

                await _productRepository.UpdateAsync(updatedProduct);

                return Ok(new
                {
                    success = true,
                    message = "Product updated successfully",
                    data = updatedProduct
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== ASSIGN PRODUCT TO VEHICLE AND DRIVER =====
        /// <summary>
        /// Assign a product to a vehicle and its driver
        /// </summary>
        [HttpPost("{id}/assign")]
        [Authorize]
        public async Task<IActionResult> AssignProduct(int id, [FromBody] Product request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid product ID" });

                if (request == null || request.VehicleId <= 0 || request.DriverId <= 0)
                    return BadRequest(new { success = false, message = "Vehicle ID and Driver ID are required" });

                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return NotFound(new { success = false, message = "Product not found" });

                await _productRepository.AssignProductToVehicleAsync(id, request.VehicleId, request.DriverId);

                return Ok(new
                {
                    success = true,
                    message = "Product assigned to vehicle and driver successfully",
                    data = new { ProductId = id, Status = "Assigned" }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== UNASSIGN PRODUCT FROM VEHICLE AND DRIVER =====
        /// <summary>
        /// Unassign a product from vehicle and driver
        /// </summary>
        [HttpPost("{id}/unassign")]
        [Authorize]
        public async Task<IActionResult> UnassignProduct(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid product ID" });

                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return NotFound(new { success = false, message = "Product not found" });

                await _productRepository.UnassignProductAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Product unassigned from vehicle and driver successfully",
                    data = new { ProductId = id, Status = "Unassigned" }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== UPDATE PRODUCT STATUS ONLY =====
        /// <summary>
        /// Update only the product status (useful for tracking delivery progress)
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] Product request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid product ID" });

                if (request == null || string.IsNullOrWhiteSpace(request.Status))
                    return BadRequest(new { success = false, message = "Status is required" });

                var validStatuses = new[] { "Unassigned", "Assigned", "InTransit", "Delivered", "Returned" };
                if (!validStatuses.Contains(request.Status))
                    return BadRequest(new { success = false, message = "Invalid status" });

                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return NotFound(new { success = false, message = "Product not found" });

                await _productRepository.UpdateProductStatusAsync(id, request.Status);

                return Ok(new
                {
                    success = true,
                    message = "Product status updated successfully",
                    data = new { ProductId = id, Status = request.Status }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== DELETE PRODUCT =====
        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid product ID" });

                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return NotFound(new { success = false, message = "Product not found" });

                await _productRepository.DeleteAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Product deleted successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}