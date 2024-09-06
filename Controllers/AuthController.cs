using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GenericMVCApp.Interfaces;
using GenericMVCApp.DTOs;
using GenericMVCApp.Models;
using GenericMVCApp.ServiceModels;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace GenericMVCApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly CookieSettings _cookieSettings;
        public AuthController(IAuthRepository authRepository, IConfiguration configuration, IOptions<CookieSettings> cookieSettings)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _cookieSettings = cookieSettings.Value;
        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(AuthDTO authDto)
        {
            var user = await _authRepository.GetUserByEmailAsync(authDto.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(authDto.Password, user.PasswordHash))
            {
                var roles = await _authRepository.GetUserRole(user.Id);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                };

                // Add multiple roles as claims
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                var isPersistent = true;
                var expiryTime = 60;
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = isPersistent,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(expiryTime)
                };

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Check if the user has the "Admin" role using the claims object
                if (roles.Any(r => r.Name == "Admin"))
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Auth");
        }
        [HttpGet]
        public async Task<IActionResult> Register(){
            var Roles = await _authRepository.GetAllRoles();
            // Create a SelectList for the dropdown
            ViewBag.Roles = new SelectList(Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email already exists.");
                return View();
            }
            
            registerDto.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.PasswordHash);
            registerDto.CreatedDate = DateTime.Now;
            registerDto.UpdatedDate = DateTime.Now;

            int userId = await _authRepository.RegisterUserAsync(registerDto);

            // Assign role to the user
            await _authRepository.AssignRoleToUser(userId, registerDto.Role);

            return RedirectToAction("Login", "Auth");
        }
    }
}
