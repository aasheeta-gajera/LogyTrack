using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogyTrackAPI.Models;
using LogyTrackAPI.Infrastructure.Data.Drivers;

namespace LogyTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;

        public DriversController(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        // ===== GET ALL DRIVERS =====
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var drivers = await _driverRepository.GetAllAsync();
                return Ok(new
                {
                    success = true,
                    message = "Drivers retrieved successfully",
                    data = drivers,
                    count = drivers.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET DRIVER BY ID =====
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid driver ID" });

                var driver = await _driverRepository.GetByIdAsync(id);
                if (driver == null)
                    return NotFound(new { success = false, message = "Driver not found" });

                return Ok(new
                {
                    success = true,
                    message = "Driver retrieved successfully",
                    data = driver
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET DRIVERS WITH VEHICLE COUNT =====
        [HttpGet("dashboard/summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDriversSummary()
        {
            try
            {
                var drivers = await _driverRepository.GetDriversWithVehiclesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Drivers summary retrieved",
                    data = drivers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET ACTIVE DRIVERS =====
        [HttpGet("active/all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveDrivers()
        {
            try
            {
                var drivers = await _driverRepository.GetActiveDriversAsync();
                return Ok(new
                {
                    success = true,
                    message = "Active drivers retrieved",
                    data = drivers,
                    count = drivers.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== CREATE DRIVER - DAPPER =====
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Driver request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.FullName))
                    return BadRequest(new { success = false, message = "All fields required" });

                var newDriver = new Driver
                {
                    FullName = request.FullName.Trim(),
                    PhoneNumber = request.PhoneNumber.Trim(),
                    LicenseNumber = request.LicenseNumber.Trim(),
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                var id = await _driverRepository.CreateAsync(newDriver);
                newDriver.DriverId = id;

                return CreatedAtAction(nameof(GetById), new { id }, new
                {
                    success = true,
                    message = "Driver created successfully",
                    data = newDriver
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== UPDATE DRIVER - DAPPER =====
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Driver request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid driver ID" });

                var existingDriver = await _driverRepository.GetByIdAsync(id);
                if (existingDriver == null)
                    return NotFound(new { success = false, message = "Driver not found" });

                var updatedDriver = new Driver
                {
                    DriverId = id,
                    FullName = request.FullName.Trim(),
                    PhoneNumber = request.PhoneNumber.Trim(),
                    LicenseNumber = request.LicenseNumber.Trim(),
                    IsActive = request.IsActive,
                    CreatedDate = existingDriver.CreatedDate,
                    UpdatedDate = DateTime.Now
                };

                await _driverRepository.UpdateAsync(updatedDriver);

                return Ok(new
                {
                    success = true,
                    message = "Driver updated successfully",
                    data = updatedDriver
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== DELETE DRIVER - DAPPER =====
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid driver ID" });

                var driver = await _driverRepository.GetByIdAsync(id);
                if (driver == null)
                    return NotFound(new { success = false, message = "Driver not found" });

                await _driverRepository.DeleteAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Driver deleted successfully",
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