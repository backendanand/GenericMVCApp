using GenericMVCApp.DTOs;
using GenericMVCApp.Models;
using GenericMVCApp.ServiceModels;

namespace GenericMVCApp.Interfaces
{
    public interface IAuthRepository
    {
        Task AssignRoleToUser(int userId, int roleId);
        Task<IEnumerable<Role>> GetAllRoles();
        Task<User> GetUserByEmailAsync(string username);
        Task<IEnumerable<Role>> GetUserRole(Int64 userId);
        Task<int> RegisterUserAsync(RegisterDTO registerDto);
    }
}
