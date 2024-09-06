using Dapper;
using GenericMVCApp.Data;
using GenericMVCApp.DTOs;
using GenericMVCApp.Interfaces;
using GenericMVCApp.Models;
using GenericMVCApp.ServiceModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GenericMVCApp.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DapperContext _context;
        public AuthRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "SELECT * FROM Users WHERE Email = @Email";
                return await connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
            }
        }

        public async Task<IEnumerable<Role>> GetUserRole(long userId)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = @"SELECT R.* FROM roles R
                         INNER JOIN user_roles UR ON R.Id = UR.RoleId
                         WHERE UR.UserId = @UserId";
                return await connection.QueryAsync<Role>(query, new { UserId = userId });
            }
        }

        public async Task AssignRoleToUser(int userId, int roleId)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "INSERT INTO user_roles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                await connection.ExecuteAsync(query, new { UserId = userId, RoleId = roleId });
            }
        }

        // Remove a role from a user
        public async Task RemoveRoleFromUser(int userId, int roleId)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "DELETE FROM user_roles WHERE UserId = @UserId AND RoleId = @RoleId";
                await connection.ExecuteAsync(query, new { UserId = userId, RoleId = roleId });
            }
        }

        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT Id, Name FROM roles";
                return await connection.QueryAsync<Role>(query);
            }
        }

        public async Task<int> RegisterUserAsync(RegisterDTO registerDto)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "INSERT INTO Users (Name, Email, PasswordHash, CreatedDate, UpdatedDate) VALUES (@Name, @Email, @PasswordHash, @CreatedDate, @UpdatedDate);SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.QuerySingleAsync<int>(query, registerDto);
            }
        }
    }
}
