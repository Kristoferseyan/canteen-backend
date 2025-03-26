using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApiLibrary.Dto;

namespace CanteenWebApiLibrary.Services
{
    public interface IOrderService
    {
        Task<ApiResponseMessage<OrderDto>> CreateOrUpdateOrder(OrderDto orderDto, bool isStaff);
        Task<ApiResponseMessage<bool>> DeleteOrder(Guid id, Guid userId, bool isStaff);
        Task<IEnumerable<OrderDto>> GetAllOrders(Guid userId, bool isStaff);
        Task<ApiResponseMessage<OrderDto>> GetOrderById(Guid id, Guid userId, bool isStaff);
        Task<PaginatedOrdersDto> GetOrdersByStatus(string status, Guid userId, bool isStaff, int pageNumber, int pageSize);
        Task<ApiResponseMessage<List<OrderSummaryDto>>> GetOrderSummaries(Guid userId,string orderStatus ,int pageNumber = 1, int pageSize = 20);
        Task<ApiResponseMessage<List<OrderDto>>> GetCompletedOrdersByUserId(Guid userId);
        
    }

}
