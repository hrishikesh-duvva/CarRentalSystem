using CarRentalSystem.Models;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CarRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CarRentalService _carRentalService;

        public CarController(CarRentalService carRentalService)
        {
            _carRentalService = carRentalService;
        }

        // GET /api/car - Get available cars (No authentication needed)
        [HttpGet]
        public async Task<IActionResult> GetAvailableCars()
        {
            var cars = await _carRentalService.GetAvailableCars();
            return Ok(cars);
        }

        // POST /api/car - Add a car to the system (Only accessible to authenticated users, e.g., Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Only Admins can add cars
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            if (ModelState.IsValid)
            {
                await _carRentalService.AddCar(car);
                return CreatedAtAction(nameof(GetAvailableCars), new { id = car.Id }, car);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("{carId}/rent")]
        [Authorize]  // Ensure the user is authenticated
        public async Task<IActionResult> RentCar(int carId)
        {
            // Retrieve user's email and name from the JWT token
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User information not found in token.");
            }

            var result = await _carRentalService.RentCar(carId, userEmail, userName);

            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(result.message);
        }
    

        // PUT /api/car/{id} - Update car details (Only accessible to authenticated users, e.g., Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Only Admins can update cars
        public async Task<IActionResult> UpdateCar(int id, [FromBody] Car car)
        {
            // Logic to update car info
            return Ok("Car updated successfully");
        }

        // DELETE /api/car/{id} - Delete a car from the system (Only accessible to authenticated users, e.g., Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Only Admins can delete cars
        public async Task<IActionResult> DeleteCar(int id)
        {
            // Logic to delete car
            return Ok("Car deleted successfully");
        }
    }
}
