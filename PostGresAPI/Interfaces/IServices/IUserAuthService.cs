using System.Threading.Tasks;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services
{
    public interface IUserAuthService
    {
        Task<(bool Success, string? Error, UserAuthResultDto? Result)> Register(UserRegisterDto registerDto);
        Task<(bool Success, string? Error, UserAuthResultDto? Result)> Login(UserLoginDto loginDto);
    }
}
