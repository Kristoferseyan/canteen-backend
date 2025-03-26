using CanteenWebApi.Entities;
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;

    public interface IUserService
    {
        Task<UserDto?> AuthenticateUser(string username, string password);
        Task<ApiResponseMessage<UserDto>> CreateOrUpdateUserDto(UserDto userDto);
        Task<List<User>> DisplayAllUsers();
        Task<IEnumerable<UserRoleDto>> DisplayUserRole();
        Task<ApiResponseMessage<UserDto>> UpdateUserRoleAsync(Guid userId, string newRoleId);
        Task<ApiResponseMessage<UserDto>> GetUserById(Guid id);
    }