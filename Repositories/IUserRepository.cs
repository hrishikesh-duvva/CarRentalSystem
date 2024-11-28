using CarRentalSystem.Models;

namespace CarRentalSystem.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int id);
        Task AddUser(User user);

        Task<User> ValidateUserCredentials(string email, string password);
    }
}
