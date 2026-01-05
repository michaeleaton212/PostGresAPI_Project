using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PostGresAPI.Contracts;
using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services
{
    public class UserAuthService : IUserAuthService
    {
        private readonly IUserRepository _userRepo;

        public UserAuthService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<(bool Success, string? Error, UserAuthResultDto? Result)> Register(UserRegisterDto registerDto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(registerDto.UserName))
                return (false, "UserName is required.", null);

            if (string.IsNullOrWhiteSpace(registerDto.Email))
                return (false, "Email is required.", null);

            if (string.IsNullOrWhiteSpace(registerDto.Password))
                return (false, "Password is required.", null);

            if (registerDto.Password.Length < 6)
                return (false, "Password must be at least 6 characters long.", null);

            // Check if user already exists
            var existing = await _userRepo.GetByUserNameOrEmail(registerDto.UserName);
            if (existing != null)
                return (false, "User with this username already exists.", null);

            existing = await _userRepo.GetByUserNameOrEmail(registerDto.Email);
            if (existing != null)
                return (false, "User with this email already exists.", null);

            // Create user entity
            var user = new User(registerDto.UserName, registerDto.Email, registerDto.Phone ?? "");
            
            // Hash password
            var passwordHash = HashPassword(registerDto.Password);
            user.SetPasswordHash(passwordHash);

            // Save to database
            var createdUser = await _userRepo.AddUserEntity(user);

            // Return result
            var result = new UserAuthResultDto(
                createdUser.Id,
                createdUser.UserName,
                createdUser.Email
            );

            return (true, null, result);
        }

        public async Task<(bool Success, string? Error, UserAuthResultDto? Result)> Login(UserLoginDto loginDto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(loginDto.UserNameOrEmail))
                return (false, "UserName or Email is required.", null);

            if (string.IsNullOrWhiteSpace(loginDto.Password))
                return (false, "Password is required.", null);

            // Find user
            var user = await _userRepo.GetByUserNameOrEmail(loginDto.UserNameOrEmail);
            if (user == null)
                return (false, "Invalid credentials.", null);

            // Verify password
            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                return (false, "Invalid credentials.", null);

            // Return result
            var result = new UserAuthResultDto(
                user.Id,
                user.UserName,
                user.Email
            );

            return (true, null, result);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}
