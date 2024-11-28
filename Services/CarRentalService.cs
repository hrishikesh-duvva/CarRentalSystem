using CarRentalSystem.Models;
using CarRentalSystem.Repositories;
using Microsoft.Extensions.Logging;

namespace CarRentalSystem.Services
{
    public class CarRentalService
    {
        private readonly ICarRepository _carRepository;
        private readonly ILogger<CarRentalService> _logger;
        private readonly EmailService _emailService;
        public CarRentalService(ICarRepository carRepository, EmailService emailService, ILogger<CarRentalService> logger)
        {
            _carRepository = carRepository;
            _emailService = emailService;
            _logger = logger;
        }

        // Add a new car 
        public async Task<string> AddCar(Car car)
        {
            try
            {
                // Check if car already exists
                var existingCar = await _carRepository.GetCarById(car.Id);
                if (existingCar != null)
                {
                    _logger.LogWarning($"Car with ID {car.Id} already exists.");
                    return "Car with this ID already exists.";
                }

                await _carRepository.AddCar(car);
                _logger.LogInformation($"Car with ID {car.Id} added successfully.");
                return "Car added successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding car: {ex.Message}");
                return "Error adding car.";
            }
        }

        public async Task<(bool success, string message)> RentCar(int carId, string userEmail, string userName)
        {
            try
            {
                var car = await _carRepository.GetCarById(carId);
                if (car == null)
                {
                    _logger.LogWarning($"Car with ID {carId} not found.");
                    return (false, "Car not found.");
                }

                if (!car.IsAvailable)
                {
                    _logger.LogWarning($"Car with ID {carId} is already rented.");
                    return (false, "Car is not available.");
                }

                // Mark the car as rented
                await _carRepository.UpdateCarAvailability(carId, false);

                // Send email notification
                var subject = "Car Rental Confirmation";
                var message = $"Dear {userName},\n\n" +
                              $"You have successfully rented the car: {car.Make} {car.Model}.\n\n" +
                              $"Rental Details:\n" +
                              $"- Car ID: {carId}\n" +
                              $"- Make & Model: {car.Make} {car.Model}\n" +
                              $"- Rental Date: {DateTime.Now}\n\n" +
                              "Thank you for using our service!";

                await _emailService.SendEmailAsync(userEmail, userName, subject, message);

                // Log success
                _logger.LogInformation($"Car with ID {carId} rented successfully by user {userEmail}.");
                return (true, "Car rented successfully. A confirmation email has been sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error renting car with ID {carId}: {ex.Message}");
                return (false, "An error occurred while renting the car. Please try again later.");
            }
        }

        // Get all available cars
        public async Task<IEnumerable<Car>> GetAvailableCars()
        {
            try
            {
                return await _carRepository.GetAvailableCars();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving available cars: {ex.Message}");
                return new List<Car>(); // Return an empty list in case of error
            }
        }
    }
}
