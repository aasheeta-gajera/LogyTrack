
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogyTrackAPI.Models;
using LogyTrackAPI.Infrastructure.Data.Vehiclas;

namespace LogyTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehiclesController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        // ===== GET ALL VEHICLES =====
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var vehicles = await _vehicleRepository.GetVehiclesWithDriverAsync();
                return Ok(new
                {
                    success = true,
                    message = "Vehicles retrieved successfully",
                    data = vehicles,
                    count = vehicles.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET VEHICLE BY ID =====
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid vehicle ID" });

                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    return NotFound(new { success = false, message = "Vehicle not found" });

                return Ok(new
                {
                    success = true,
                    message = "Vehicle retrieved successfully",
                    data = vehicle
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== GET VEHICLES BY STATUS =====
        [HttpGet("by-status/{status}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                var validStatuses = new[] { "Available", "OnTrip", "Maintenance" };
                if (!validStatuses.Contains(status))
                    return BadRequest(new { success = false, message = "Invalid status" });

                var vehicles = await _vehicleRepository.GetByStatusAsync(status);
                return Ok(new
                {
                    success = true,
                    message = $"Vehicles with status '{status}' retrieved",
                    data = vehicles,
                    count = vehicles.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== CREATE VEHICLE - DAPPER =====
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Vehicle request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.VehicleNumber))
                    return BadRequest(new { success = false, message = "All fields required" });

                var newVehicle = new Vehicle
                {
                    VehicleNumber = request.VehicleNumber.Trim(),
                    Model = request.Model.Trim(),
                    CapacityKg = request.CapacityKg,
                    DriverId = request.DriverId,
                    Status = "Available",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                var id = await _vehicleRepository.CreateAsync(newVehicle);
                newVehicle.VehicleId = id;

                return CreatedAtAction(nameof(GetById), new { id }, new
                {
                    success = true,
                    message = "Vehicle created successfully",
                    data = newVehicle
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== UPDATE VEHICLE - DAPPER =====
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Vehicle request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid vehicle ID" });

                var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
                if (existingVehicle == null)
                    return NotFound(new { success = false, message = "Vehicle not found" });

                var updatedVehicle = new Vehicle
                {
                    VehicleId = id,
                    VehicleNumber = request.VehicleNumber.Trim(),
                    Model = request.Model.Trim(),
                    CapacityKg = request.CapacityKg,
                    DriverId = request.DriverId,
                    Status = request.Status,
                    CreatedDate = existingVehicle.CreatedDate,
                    UpdatedDate = DateTime.Now
                };

                await _vehicleRepository.UpdateAsync(updatedVehicle);

                return Ok(new
                {
                    success = true,
                    message = "Vehicle updated successfully",
                    data = updatedVehicle
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== DELETE VEHICLE - DAPPER =====
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid vehicle ID" });

                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    return NotFound(new { success = false, message = "Vehicle not found" });

                await _vehicleRepository.DeleteAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Vehicle deleted successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== ASSIGN DRIVER - DAPPER =====
        [HttpPut("{id}/assign-driver/{driverId}")]
        [Authorize]
        public async Task<IActionResult> AssignDriver(int id, int driverId)
        {
            try
            {
                if (id <= 0 || driverId <= 0)
                    return BadRequest(new { success = false, message = "Invalid IDs" });

                var assigned = await _vehicleRepository.AssignDriverAsync(id, driverId);
                if (assigned <= 0)
                    return NotFound(new { success = false, message = "Vehicle not found" });

                return Ok(new
                {
                    success = true,
                    message = "Driver assigned successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== UNASSIGN DRIVER - DAPPER =====
        [HttpPut("{id}/unassign-driver")]
        [Authorize]
        public async Task<IActionResult> UnassignDriver(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid vehicle ID" });

                var unassigned = await _vehicleRepository.UnassignDriverAsync(id);
                if (unassigned <= 0)
                    return NotFound(new { success = false, message = "Vehicle not found" });

                return Ok(new
                {
                    success = true,
                    message = "Driver unassigned successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== UPDATE STATUS - DAPPER =====
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] dynamic request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid vehicle ID" });

                string newStatus = request?.Status ?? request?.status;
                var updated = await _vehicleRepository.UpdateStatusAsync(id, newStatus);
                if (updated <= 0)
                    return NotFound(new { success = false, message = "Vehicle not found" });

                return Ok(new
                {
                    success = true,
                    message = "Vehicle status updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}