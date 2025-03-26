using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;


namespace CanteenWebApiLibrary.Services
{
    public interface IReceiptServices
    {
        Task<ApiResponseMessage<ReceiptDto>> CreateReceipt(ReceiptDto dto);
        Task<ApiResponseMessage<ReceiptDto>> GetReceiptByOrderId(Guid orderId);
    }

}
