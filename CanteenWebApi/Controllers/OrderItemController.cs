using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        private Guid GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        public async Task<IActionResult> CreateOrUpdateOrderItem([FromBody] OrderItemDto orderItemDto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();

                var result = await _orderItemService.CreateOrUpdateOrderItem(orderItemDto, userId, isStaff);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> GetAllOrderItems()
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();
                var result = await _orderItemService.GetAllOrderItems(userId, isStaff);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> GetOrderItemById(Guid id)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();

                var result = await _orderItemService.GetOrderItemById(id, userId, isStaff);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> DeleteOrderItem(Guid id)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();

                var result = await _orderItemService.DeleteOrderItem(id, userId, isStaff);
                return result.IsSuccess ? Ok(result) : NotFound(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpGet("order/{orderId}")]
        [Authorize(Roles = "employee, staff")]
        public async Task<IActionResult> GetOrderItemsByOrderId(Guid orderId)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var isStaff = IsUserStaff();
                var result = await _orderItemService.GetOrderItemsByOrderId(orderId, userId, isStaff);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
