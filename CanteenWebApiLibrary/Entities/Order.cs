using System;
using System.Collections.Generic;

namespace CanteenWebApi.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public decimal? TlAmnt { get; set; }

    public DateTime OrderDate { get; set; }

    public Guid UserId { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserPayment> UserPayments { get; set; } = new List<UserPayment>();
}
