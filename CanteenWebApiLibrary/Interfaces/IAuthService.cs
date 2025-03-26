using CanteenWebApi.Entities;
using CanteenWebApiLibrary.Dto;


namespace CanteenWebApiLibrary.Services{
public interface IAuthService
    {
        Task<string?> AuthenticateUser(LoginRequestDto loginDto);
        Task<User> GetUserByUsernameAsync(string username);
        Task<List<string>> GetUserRolesAsync(Guid userId);
    }
}
