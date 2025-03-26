using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetRoles")]
        public async Task<ActionResult<ApiResponseMessage<List<RoleDto>>>> GetRoles()
        {
            var response = await _roleService.GetRolesAsync(); 
            return Ok(new ApiResponseMessage<List<RoleDto>>
            {
                Data = response,
                IsSuccess = true,
                Message = "Roles fetched successfully"
            });
        }
    }
}
