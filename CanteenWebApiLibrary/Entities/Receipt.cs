using System;
using System.Collections.Generic;

namespace CanteenWebApi.Entities;

public partial class Receipt
{
    public Guid Id { get; set; }

    public string ReceiptNumber { get; set; } = null!;

    public Guid OrderId { get; set; }

    public Guid PaymentId { get; set; }

    public DateTime? IssuedDate { get; set; }

    public decimal TotalAmount { get; set; }

    public bool? Voided { get; set; }

    public string? VoidReason { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual UserPayment Payment { get; set; } = null!;
}
