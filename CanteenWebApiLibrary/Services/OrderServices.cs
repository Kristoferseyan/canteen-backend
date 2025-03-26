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
    public class OrderService : IOrderService
    {
        private readonly CanteenDbContext _context;

        public OrderService(CanteenDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseMessage<OrderDto>> CreateOrUpdateOrder(OrderDto orderDto, bool isStaff)
        {
            var response = new ApiResponseMessage<OrderDto>();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (orderDto.Id == null)
                {
                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        TlAmnt = orderDto.TlAmnt ?? 0,
                        OrderDate = orderDto.OrderDate,
                        UserId = orderDto.UserId,
                        Status = orderDto.Status,
                        PaymentMethod = orderDto.PaymentMethod ?? "Cash"
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    var paymentStatus = order.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)
                        ? "Paid"
                        : (order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase) ||
                        order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                        order.Status.Equals("Ready", StringComparison.OrdinalIgnoreCase))
                            ? "Unpaid"
                            : (order.PaymentMethod.Equals("SD", StringComparison.OrdinalIgnoreCase) ? "Unpaid" : "Paid");

                    var userPayment = new UserPayment
                    {
                        Id = Guid.NewGuid(),
                        UserId = order.UserId,
                        OrderId = order.Id,
                        Amount = (decimal)order.TlAmnt,
                        PaymentStatus = paymentStatus
                    };

                    _context.UserPayments.Add(userPayment);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    response.Data = new OrderDto
                    {
                        Id = order.Id,
                        TlAmnt = order.TlAmnt,
                        OrderDate = order.OrderDate,
                        UserId = order.UserId,
                        Status = order.Status,
                        PaymentMethod = order.PaymentMethod
                    };

                    response.IsSuccess = true;
                    response.Message = "Order created successfully with payment entry";
                }
                else
                {
                    var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderDto.Id);

                    if (order == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Order not found.";
                        return response;
                    }

                    if (!isStaff && order.UserId != orderDto.UserId)
                    {
                        response.IsSuccess = false;
                        response.Message = "Unauthorized access to update order.";
                        return response;
                    }

                    bool paymentMethodChanged = order.PaymentMethod != orderDto.PaymentMethod;
                    bool amountChanged = order.TlAmnt != orderDto.TlAmnt;
                    bool statusChanged = order.Status != orderDto.Status;

                    order.TlAmnt = orderDto.TlAmnt ?? 0;
                    order.OrderDate = orderDto.OrderDate;
                    order.Status = orderDto.Status;
                    order.PaymentMethod = orderDto.PaymentMethod ?? order.PaymentMethod;

                    await _context.SaveChangesAsync();

                    var userPayment = await _context.UserPayments.FirstOrDefaultAsync(p => p.OrderId == order.Id);

                    if (userPayment != null)
                    {
                        if (paymentMethodChanged || amountChanged || statusChanged)
                        {
                            userPayment.Amount = (decimal)order.TlAmnt;

                            userPayment.PaymentStatus = (order.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                                                        !order.PaymentMethod.Equals("SD", StringComparison.OrdinalIgnoreCase))
                                ? "Paid"
                                : (order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase) ||
                                order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                                order.Status.Equals("Ready", StringComparison.OrdinalIgnoreCase))
                                    ? "Unpaid"
                                    : (order.PaymentMethod.Equals("SD", StringComparison.OrdinalIgnoreCase) ? "Unpaid" : "Paid");
                            userPayment.UpdatedDate = DateTime.UtcNow;
                            _context.UserPayments.Update(userPayment);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var newUserPayment = new UserPayment
                        {
                            Id = Guid.NewGuid(),
                            UserId = order.UserId,
                            OrderId = order.Id,
                            Amount = (decimal)order.TlAmnt,
                            PaymentStatus = order.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)
                                ? "Paid"
                                : (order.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase) ||
                                order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                                order.Status.Equals("Ready", StringComparison.OrdinalIgnoreCase))
                                    ? "Unpaid"
                                    : (order.PaymentMethod.Equals("SD", StringComparison.OrdinalIgnoreCase) ? "Unpaid" : "Paid")
                        };

                        _context.UserPayments.Add(newUserPayment);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    response.Data = orderDto;
                    response.IsSuccess = true;
                    response.Message = "Order updated successfully with payment changes";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<IEnumerable<OrderDto>> GetAllOrders(Guid userId, bool isStaff)
        {
            var ordersQuery = _context.Orders
                .Where(o => isStaff || o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TlAmnt = o.TlAmnt,
                    OrderDate = o.OrderDate,
                    UserId = o.UserId,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod
                });

            return await ordersQuery.ToListAsync();
        }

        public async Task<ApiResponseMessage<OrderDto>> GetOrderById(Guid id, Guid userId, bool isStaff)
        {
            var response = new ApiResponseMessage<OrderDto>();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || (!isStaff && order.UserId != userId))
            {
                response.IsSuccess = false;
                response.Message = "Order not found or unauthorized access";
                return response;
            }

            response.Data = new OrderDto
            {
                Id = order.Id,
                TlAmnt = order.TlAmnt,
                OrderDate = order.OrderDate,
                UserId = order.UserId,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod
            };

            response.IsSuccess = true;
            response.Message = "Order retrieved successfully";
            return response;
        }

        public async Task<ApiResponseMessage<List<OrderDto>>> GetCompletedOrdersByUserId(Guid userId)
        {
            var response = new ApiResponseMessage<List<OrderDto>>();

            try
            {
                var completedOrders = await _context.Orders
                    .Where(o => o.UserId == userId && (o.Status == "Completed" || o.Status == "completed"))
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                if (!completedOrders.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No completed orders found for this user";
                    return response;
                }

                response.Data = completedOrders.Select(order => new OrderDto
                {
                    Id = order.Id,
                    TlAmnt = order.TlAmnt,
                    OrderDate = order.OrderDate,
                    UserId = order.UserId,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Completed orders retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error retrieving completed orders: {ex.Message}";
            }

            return response;
        }
        public async Task<ApiResponseMessage<bool>> DeleteOrder(Guid id, Guid userId, bool isStaff)
        {
            var response = new ApiResponseMessage<bool>();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || (!isStaff && order.UserId != userId))
            {
                response.IsSuccess = false;
                response.Message = "Order not found or unauthorized access";
                return response;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.IsSuccess = true;
            response.Message = "Order deleted successfully";
            return response;
        }
        public async Task<ApiResponseMessage<List<OrderSummaryDto>>> GetOrderSummaries(Guid userId, string orderStatus, int pageNumber = 1, int pageSize = 20)
        {
            var response = new ApiResponseMessage<List<OrderSummaryDto>>();

            var ordersQuery = _context.Orders.AsQueryable();

            ordersQuery = ordersQuery.Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(orderStatus))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == orderStatus);
            }

            ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);
            var totalRecords = await ordersQuery.CountAsync();

            var orders = await ordersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!orders.Any())
            {
                response.IsSuccess = false;
                response.Message = "No orders found";
                return response;
            }

            response.Data = orders.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderCode = o.Id.ToString().Substring(0, 6),
                OrderDate = o.OrderDate,
                TlAmnt = o.TlAmnt,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod
            }).ToList();

            response.IsSuccess = true;
            response.Message = "Order summaries retrieved successfully";
            response.TotalRecords = totalRecords;
            response.PageNumber = pageNumber;
            response.PageSize = pageSize;

            return response;
        }
        public async Task<PaginatedOrdersDto> GetOrdersByStatus(string status, Guid userId, bool isStaff, int pageNumber, int pageSize)
        {
            var query = _context.Orders
                .Where(o => o.Status == status && (isStaff || o.UserId == userId));

            int totalRecords = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TlAmnt = o.TlAmnt,
                    OrderDate = o.OrderDate,
                    UserId = o.UserId,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod
                })
                .ToListAsync();

            return new PaginatedOrdersDto
            {
                Orders = orders,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }


    }
}
