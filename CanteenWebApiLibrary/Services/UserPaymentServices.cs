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
    public static class PaymentStatus
    {
        public const string Paid = "Paid";
        public const string Unpaid = "Unpaid";
    }

    public class UserPaymentsService : IUserPaymentsService
    {
        private readonly CanteenDbContext _context;

        public UserPaymentsService(CanteenDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseMessage<UserPaymentsDto>> CreatePayment(UserPaymentsDto dto)
        {
            var response = new ApiResponseMessage<UserPaymentsDto>();

            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
                var orderExists = await _context.Orders.AnyAsync(o => o.Id == dto.OrderId);

                if (!userExists || !orderExists)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid user or order.";
                    return response;
                }

                var existingPayment = await _context.UserPayments
                    .AnyAsync(p => p.OrderId == dto.OrderId && p.UserId == dto.UserId && p.PaymentStatus == PaymentStatus.Unpaid);

                if (existingPayment)
                {
                    response.IsSuccess = false;
                    response.Message = "An unpaid payment already exists for this order.";
                    return response;
                }

                var payment = new UserPayment
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    OrderId = dto.OrderId,
                    Amount = dto.Amount ?? 0,
                    PaymentStatus = PaymentStatus.Unpaid
                };

                _context.UserPayments.Add(payment);
                await _context.SaveChangesAsync();

                response.Data = dto;
                response.IsSuccess = true;
                response.Message = "Payment recorded successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponseMessage<List<UserPaymentsDto>>> GetPaymentsByUser(Guid userId)
        {
            var response = new ApiResponseMessage<List<UserPaymentsDto>>();

            var payments = await _context.UserPayments
                .Where(p => p.UserId == userId)
                .Select(p => new UserPaymentsDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    PaymentStatus = p.PaymentStatus
                })
                .ToListAsync();

            if (!payments.Any())
            {
                response.IsSuccess = false;
                response.Message = "No payment records found.";
                return response;
            }

            response.Data = payments;
            response.IsSuccess = true;
            response.Message = "Payments retrieved successfully";
            return response;
        }

        public async Task<ApiResponseMessage<decimal>> GetUnpaidSDBalance(Guid userId)
        {
            var response = new ApiResponseMessage<decimal>();

                decimal balance = await _context.UserPayments
                    .Join(_context.Orders,
                            payment => payment.OrderId,
                            order => order.Id,
                            (payment, order) => new { payment, order })
                    .Where(po => po.payment.UserId == userId
                            && po.payment.PaymentStatus == PaymentStatus.Unpaid
                            && po.order.PaymentMethod == "SD"
                        && (po.order.Status == "Completed" || po.order.Status == "completed"))
                    .SumAsync(po => po.payment.Amount);

            response.Data = balance;
            response.IsSuccess = true;
            response.Message = "Unpaid salary deduction balance retrieved";
            return response;
        }

        public async Task<ApiResponseMessage<decimal>> GetUnpaidSDBalanceByName(string name)
        {
            var response = new ApiResponseMessage<decimal>();

            try
            {
                var user = await _context.Users
                    .Where(u => (u.FirstName + " " + u.LastName).ToLower() == name.ToLower())
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                decimal balance = await _context.UserPayments
                    .Join(_context.Orders,
                            payment => payment.OrderId,
                            order => order.Id,
                            (payment, order) => new { payment, order })
                    .Where(po => po.payment.UserId == user.Id
                            && po.payment.PaymentStatus == PaymentStatus.Unpaid
                            && po.order.PaymentMethod == "SD"
                        && (po.order.Status == "Completed" || po.order.Status == "completed"))
                    .SumAsync(po => po.payment.Amount);

                response.Data = balance;
                response.IsSuccess = true;
                response.Message = "Unpaid salary deduction balance retrieved";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                response.Data = 0;
            }

            return response;
        }

        public async Task<ApiResponseMessage<bool>> MarkPaymentsAsCompleted(Guid userId)
        {
            var response = new ApiResponseMessage<bool>();

            var payments = await _context.UserPayments
                .Where(p => p.UserId == userId && p.PaymentStatus == PaymentStatus.Unpaid)
                .ToListAsync();

            if (!payments.Any())
            {
                response.IsSuccess = false;
                response.Message = "No pending payments found.";
                return response;
            }

            foreach (var payment in payments)
            {
                payment.PaymentStatus = PaymentStatus.Paid;
            }

            await _context.SaveChangesAsync();

            response.Data = true;
            response.IsSuccess = true;
            response.Message = "Salary deduction applied successfully.";
            return response;
        }

        public async Task<ApiResponseMessage<List<UserPaymentDetailsDto>>> GetUserPayments()
        {
            var response = new ApiResponseMessage<List<UserPaymentDetailsDto>>();

            var payments = await (from o in _context.Orders
                                  join u in _context.Users on o.UserId equals u.Id
                                  join up in _context.UserPayments on o.Id equals up.OrderId into paymentGroup
                                  from up in paymentGroup.DefaultIfEmpty()
                                  where o.Status == "Completed"
                                  select new UserPaymentDetailsDto
                                  {
                                      Name = u.FirstName + " " + u.LastName,
                                      OrderCode = o.Id.ToString().Substring(0, 6),
                                      Amount = o.TlAmnt,
                                      PaymentMethod = o.PaymentMethod,
                                      PaymentStatus = up != null ? up.PaymentStatus : "Unpaid",
                                      CreatedDate = up.CreatedDate,
                                      UpdatedDate = up.UpdatedDate,
                                  }).ToListAsync();


            if (!payments.Any())
            {
                response.IsSuccess = false;
                response.Message = "No payment records found.";
                return response;
            }

            response.Data = payments;
            response.IsSuccess = true;
            response.Message = "Payments retrieved successfully";
            return response;
        }

        public async Task<ApiResponseMessage<List<UserPaymentDetailsDto>>> GetUserPayments(
            string paymentMethod = null,
            string paymentStatus = null,
            string name = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = new ApiResponseMessage<List<UserPaymentDetailsDto>>();

            try
            {
                var query = from o in _context.Orders
                            join u in _context.Users on o.UserId equals u.Id
                            join up in _context.UserPayments on o.Id equals up.OrderId into paymentGroup
                            from up in paymentGroup.DefaultIfEmpty()
                            where o.Status == "Completed"
                            select new UserPaymentDetailsDto
                            {
                                Name = u.FirstName + " " + u.LastName,
                                OrderCode = o.Id.ToString().Substring(0, 6),
                                Amount = o.TlAmnt,
                                PaymentMethod = o.PaymentMethod,
                                PaymentStatus = up != null && !string.IsNullOrEmpty(up.PaymentStatus)
                                    ? up.PaymentStatus.ToLower()
                                    : "unpaid",
                                CreatedDate = up.CreatedDate,
                                UpdatedDate = up.UpdatedDate
                            };

                if (!string.IsNullOrEmpty(paymentMethod) && paymentMethod != "All")
                {
                    query = query.Where(x => x.PaymentMethod.ToLower() == paymentMethod.ToLower());
                }

                if (!string.IsNullOrEmpty(paymentStatus) && paymentStatus != "All")
                {
                    if (paymentStatus.ToLower() == "unpaid")
                    {
                        query = query.Where(x => x.PaymentStatus == "unpaid");
                    }
                    else
                    {
                        query = query.Where(x => x.PaymentStatus.ToLower() == paymentStatus.ToLower());
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(name.ToLower()));
                }

                var totalRecords = await query.CountAsync();

                var payments = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!payments.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No payment records found.";
                    response.Data = new List<UserPaymentDetailsDto>();
                    return response;
                }

                response.Data = payments;
                response.IsSuccess = true;
                response.Message = "Payments retrieved successfully";
                response.TotalRecords = totalRecords;
                response.PageNumber = pageNumber;
                response.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
        public async Task<ApiResponseMessage<UserPaymentsDto>> GetPaymentByOrderId(Guid orderId)
        {
            var response = new ApiResponseMessage<UserPaymentsDto>();
            var payment = await _context.UserPayments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                response.IsSuccess = false;
                response.Message = "Payment not found.";
                return response;
            }

            response.Data = new UserPaymentsDto
            {
                Id = payment.Id,
                UserId = payment.UserId,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentStatus = payment.PaymentStatus
            };
            response.IsSuccess = true;
            response.Message = "Payment retrieved successfully";
            return response;
        }
    }
}