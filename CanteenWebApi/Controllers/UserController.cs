using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userServices;
        public UserController(IUserService userServices)
        {
            _userServices = userServices;
        }
        [HttpPost("CreateUpdateUser")]
        public async Task<ActionResult<ApiResponseMessage<UserDto>>> CreateOrUpdateUser(UserDto dto)
        {
            var response = await _userServices.CreateOrUpdateUserDto(dto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
        
        [HttpGet("DisplayAllUser")]
        public async Task<IActionResult> DisplayAllUsers()
        {
            var users = await _userServices.DisplayAllUsers();
            return Ok(users);
        }
        [HttpGet("GetUserByID/{id}")]
        public async Task<ActionResult<ApiResponseMessage<UserDto>>> GetUserById(Guid id)
        {
            var response = await _userServices.GetUserById(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("DisplayUserRole")]
        public async Task<ActionResult<ApiResponseMessage<IEnumerable<UserRoleDto>>>> UserRole()
        {
            var item = await _userServices.DisplayUserRole();
            if (item == null)
            {
                return Ok(new ApiResponseMessage<IEnumerable<UserRoleDto>>()
                {
                    Data = [],
                    Message = "No Product",
                    IsSuccess = false
                });
            }
            return Ok(new ApiResponseMessage<IEnumerable<UserRoleDto>>()
            {
                Data = item,
                Message = "Ok",
                IsSuccess = true
            });
        }
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponseMessage<UserDto>>> Login([FromBody] LoginRequestDto request)
        {
            var user = await _userServices.AuthenticateUser(request.Username, request.Password);

            if (user == null)
            {
                return Unauthorized(new ApiResponseMessage<UserDto>
                {
                    Data = null,
                    Message = "Invalid username or password",
                    IsSuccess = false
                });
            }

            return Ok(new ApiResponseMessage<UserDto>
            {
                Data = user,
                Message = "Login successful",
                IsSuccess = true
            });
        }

        [HttpPut("UpdateUserRole")]
        public async Task<ActionResult<ApiResponseMessage<UserDto>>> UpdateUserRole([FromBody] UpdateRoleRequestDto request)
        {
            var response = await _userServices.UpdateUserRoleAsync(request.UserId, request.NewRoleId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

    }
}
