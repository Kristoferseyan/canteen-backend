using CanteenWebApi.Entities;
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Services
{
    public class ReceiptServices : IReceiptServices
    {
        private readonly CanteenDbContext _context;

        public ReceiptServices(CanteenDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseMessage<ReceiptDto>> CreateReceipt(ReceiptDto dto)
        {
            var response = new ApiResponseMessage<ReceiptDto>();

            try
            {
                var receipt = new Receipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptNumber = dto.receiptNumber,
                    OrderId = dto.orderId,
                    PaymentId = dto.paymentId,
                    IssuedDate = dto.issuedDate,
                    TotalAmount = dto.totalAmount,
                    Voided = dto.voided,
                    VoidReason = dto.voidReason
                };

                _context.Receipts.Add(receipt);
                await _context.SaveChangesAsync();

                dto.Id = receipt.Id;

                response.Data = dto;
                response.IsSuccess = true;
                response.Message = "Receipt created successfully!";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ApiResponseMessage<ReceiptDto>> GetReceiptByOrderId(Guid orderId)
        {
            var response = new ApiResponseMessage<ReceiptDto>();

            try
            {
            var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.OrderId == orderId);

            if (receipt == null)
            {
                response.IsSuccess = false;
                response.Message = "Receipt not found for the given Order ID.";
                return response;
            }

            response.Data = new ReceiptDto
            {
                Id = receipt.Id,
                receiptNumber = receipt.ReceiptNumber,
                orderId = receipt.OrderId,
                paymentId = receipt.PaymentId,
                issuedDate = receipt.IssuedDate ?? DateTime.Now,
                totalAmount = receipt.TotalAmount,
                voided = receipt.Voided ?? false,
                voidReason = receipt.VoidReason
            };

            response.IsSuccess = true;
            response.Message = "Receipt retrieved successfully!";
            }
            catch (Exception ex)
            {
            response.IsSuccess = false;
            response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
    }
}
