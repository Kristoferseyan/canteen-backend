using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.ApiResponseMessage;
using CanteenWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Services
{
    public interface IOrderItemService
    {
        Task<ApiResponseMessage<OrderItemDto>> CreateOrUpdateOrderItem(OrderItemDto orderItemDto, Guid userId, bool isStaff);
        Task<ApiResponseMessage<bool>> DeleteOrderItem(Guid id, Guid userId, bool isStaff);
        Task<IEnumerable<OrderItemDto>> GetAllOrderItems(Guid userId, bool isStaff);
        Task<ApiResponseMessage<OrderItemDto>> GetOrderItemById(Guid id, Guid userId, bool isStaff);
        Task<List<ItemByOrderDto>> GetOrderItemsByOrderId(Guid orderId, Guid userId, bool isStaff);
    }

    public class OrderItemService : IOrderItemService
    {
        private readonly CanteenDbContext _context;

        public OrderItemService(CanteenDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseMessage<OrderItemDto>> CreateOrUpdateOrderItem(OrderItemDto orderItemDto, Guid userId, bool isStaff)
        {
            var response = new ApiResponseMessage<OrderItemDto>();

            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderItemDto.OrderId && (isStaff || o.UserId == userId));

                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found or unauthorized access";
                    return response;
                }

                if (orderItemDto.Id == null)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderItemDto.OrderId,
                        ItemId = orderItemDto.ItemId,
                        Quantity = orderItemDto.Quantity,
                        Price = orderItemDto.Price
                    };

                    _context.OrderItems.Add(orderItem);
                    await _context.SaveChangesAsync();

                    orderItemDto.Id = orderItem.Id;
                    response.Data = orderItemDto;
                    response.IsSuccess = true;
                    response.Message = "Order item created successfully";
                }
                else
                {
                    var orderItem = await _context.OrderItems
                        .FirstOrDefaultAsync(oi => oi.Id == orderItemDto.Id && oi.OrderId == orderItemDto.OrderId);

                    if (orderItem == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Order item not found or not associated with the specified order";
                        return response;
                    }

                    orderItem.OrderId = orderItemDto.OrderId;
                    orderItem.ItemId = orderItemDto.ItemId;
                    orderItem.Quantity = orderItemDto.Quantity;
                    orderItem.Price = orderItemDto.Price;

                    await _context.SaveChangesAsync();

                    response.Data = orderItemDto;
                    response.IsSuccess = true;
                    response.Message = "Order item updated successfully";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllOrderItems(Guid userId, bool isStaff)
        {
            IQueryable<OrderItem> query = _context.OrderItems;

            if (!isStaff)
            {
                query = query.Where(oi => oi.Order.UserId == userId);
            }

            return await query
                .Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ItemId = oi.ItemId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToListAsync();
        }

        public async Task<ApiResponseMessage<OrderItemDto>> GetOrderItemById(Guid id, Guid userId, bool isStaff)
        {
            var response = new ApiResponseMessage<OrderItemDto>();

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.Id == id && (isStaff || oi.Order.UserId == userId));

            if (orderItem == null)
            {
                response.IsSuccess = false;
                response.Message = "Order item not found or unauthorized access";
                return response;
            }

            response.Data = new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ItemId = orderItem.ItemId,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price
            };

            response.IsSuccess = true;
            response.Message = "Order item retrieved successfully";
            return response;
        }

        public async Task<ApiResponseMessage<bool>> DeleteOrderItem(Guid id, Guid userId, bool isStaff)
        {
            var response = new ApiResponseMessage<bool>();

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.Id == id && (isStaff || oi.Order.UserId == userId));

            if (orderItem == null)
            {
                response.IsSuccess = false;
                response.Message = "Order item not found or unauthorized access";
                return response;
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.IsSuccess = true;
            response.Message = "Order item deleted successfully";
            return response;
        }
        public async Task<List<ItemByOrderDto>> GetOrderItemsByOrderId(Guid orderId, Guid userId, bool isStaff)
        {
            var orderItemsQuery = from oi in _context.OrderItems
                                join i in _context.MenuItems on oi.ItemId equals i.Id
                                where oi.OrderId == orderId
                                select new ItemByOrderDto
                                {
                                    OrderId = oi.OrderId,
                                    ItemId = oi.ItemId,
                                    Quantity = oi.Quantity,
                                    Price = oi.Price,
                                    ItemName = i.ItemName
                                };

            if (!isStaff)
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null || order.UserId != userId)
                {
                    throw new UnauthorizedAccessException("Unauthorized access to this order.");
                }
            }

            return await orderItemsQuery.ToListAsync();
        }



    }
}
