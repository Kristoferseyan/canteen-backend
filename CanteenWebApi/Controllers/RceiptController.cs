using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using Microsoft.AspNetCore.Mvc;
using CanteenWebApiLibrary.Services;
using CanteenWebApi.Entities;

namespace CanteenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptServices _service; 
        
        public ReceiptController(IReceiptServices service) 
        {
            _service = service;  
        }
        
        [HttpPost("CreateReceipt")]
        public async Task<IActionResult> CreateReceipt(ReceiptDto dto)
        {
            var response = await _service.CreateReceipt(dto);
            
            if (response.IsSuccess)
                return Ok(response);
            else
                return BadRequest(response);
        }
        
        [HttpGet("GetReceipt-byOrderId")]
        public async Task<ActionResult<ReceiptDto>> GetReceiptByOrderId(Guid orderId)
        {
            var response = await _service.GetReceiptByOrderId(orderId);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

    }
}