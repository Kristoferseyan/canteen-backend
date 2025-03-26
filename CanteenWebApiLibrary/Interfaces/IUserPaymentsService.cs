
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;

namespace CanteenWebApiLibrary.Services
{
    public interface IUserPaymentsService
    {
        Task<ApiResponseMessage<UserPaymentsDto>> CreatePayment(UserPaymentsDto dto);
        Task<ApiResponseMessage<List<UserPaymentsDto>>> GetPaymentsByUser(Guid userId);
        Task<ApiResponseMessage<global::System.Decimal>> GetUnpaidSDBalance(Guid userId);
        Task<ApiResponseMessage<global::System.Boolean>> MarkPaymentsAsCompleted(Guid userId);
        Task<ApiResponseMessage<List<UserPaymentDetailsDto>>> GetUserPayments();
        Task<ApiResponseMessage<List<UserPaymentDetailsDto>>> GetUserPayments(string paymentMethod = null, string paymentStatus = null, string name = null, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponseMessage<decimal>> GetUnpaidSDBalanceByName(string name);
        Task<ApiResponseMessage<UserPaymentsDto>> GetPaymentByOrderId(Guid orderId);
    }
}

