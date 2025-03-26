using System;
using System.Collections.Generic;

namespace CanteenWebApi.Entities;

public partial class UserPayment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public virtual User User { get; set; } = null!;
}
