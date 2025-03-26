using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPaymentsController : ControllerBase
    {
        private readonly IUserPaymentsService _userPaymentsService;

        public UserPaymentsController(IUserPaymentsService userPaymentsService)
        {
            _userPaymentsService = userPaymentsService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseMessage<UserPaymentsDto>>> CreatePayment(UserPaymentsDto dto)
        {
            var response = await _userPaymentsService.CreatePayment(dto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<ApiResponseMessage<List<UserPaymentsDto>>>> GetPaymentsByUser(Guid userId)
        {
            var response = await _userPaymentsService.GetPaymentsByUser(userId);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpGet("balance/{userId}")]
        public async Task<ActionResult<ApiResponseMessage<decimal>>> GetUnpaidSDBalance(Guid userId)
        {
            var response = await _userPaymentsService.GetUnpaidSDBalance(userId);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpGet("balanceByName/{name}")]
        public async Task<ActionResult<ApiResponseMessage<decimal>>> GetUnpaidSDBalanceByName(string name)
        {
            var response = await _userPaymentsService.GetUnpaidSDBalanceByName(name);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpPut("mark-as-paid/{userId}")]
        public async Task<ActionResult<ApiResponseMessage<bool>>> MarkPaymentsAsCompleted(Guid userId)
        {
            var response = await _userPaymentsService.MarkPaymentsAsCompleted(userId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("user-payments-details")]
        public async Task<ActionResult<ApiResponseMessage<List<UserPaymentDetailsDto>>>> GetUserPayments()
        {
            var response = await _userPaymentsService.GetUserPayments();
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpGet("RetrievePayments")]
        public async Task<ActionResult<ApiResponseMessage<List<UserPaymentDetailsDto>>>> GetUserPayments(
            string paymentMethod = null,
            string paymentStatus = null,
            string name = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await _userPaymentsService.GetUserPayments(
                paymentMethod: paymentMethod,
                paymentStatus: paymentStatus,
                name: name,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            if (response.Message == "No payment records found.")
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(Guid orderId)
        {
            var response = await _userPaymentsService.GetPaymentByOrderId(orderId);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }
    }
}
