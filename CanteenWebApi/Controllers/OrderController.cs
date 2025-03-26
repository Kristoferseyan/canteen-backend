using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuthService _authService;

        public OrderController(IOrderService orderService, IAuthService authService)
        {
            _orderService = orderService;
            _authService = authService;
        }

        private Guid GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new UnauthorizedAccessException("Invalid or missing 'userId' claim.");
            }

            return userId;
        }

        private bool IsUserStaff()
        {
            return User.IsInRole("staff");
        }

        [HttpPost("createOrUpdate")]
        [Authorize(Roles = "employee, staff")]
        public async Task<ActionResult<ApiResponseMessage<OrderDto>>> CreateOrUpdateOrder(OrderDto orderDto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                orderDto.UserId = userId;
                var isStaff = IsUserStaff();

                var result = await _orderService.CreateOrUpdateOrder(orderDto, isStaff);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();
                var result = await _orderService.GetAllOrders(userId, isStaff);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();
                var result = await _orderService.GetOrderById(id, userId, isStaff);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        
        [HttpGet("by-userId")]
        public async Task<IActionResult> GetCompletedOrdersByUserId(Guid userId)
        {
            var response = await _orderService.GetCompletedOrdersByUserId(userId);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();
                var result = await _orderService.DeleteOrder(id, userId, isStaff);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("summaries")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> GetOrderSummaries(Guid userId, string orderStatus, int pageNumber = 1, int pageSize = 20)
        {
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "Page size must be between 1 and 100." });
            }

            var result = await _orderService.GetOrderSummaries(userId, orderStatus, pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("byStatus")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> GetOrdersByStatus([FromQuery] string status, int pageNumber = 1, int pageSize = 20)
        {   
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "Page size must be between 1 and 100." });
            }

            var userId = GetUserIdFromClaims();
            var isStaff = IsUserStaff();
            var orders = await _orderService.GetOrdersByStatus(status, userId, isStaff, pageNumber, pageSize);

            return Ok(orders);
        }
    }
}
