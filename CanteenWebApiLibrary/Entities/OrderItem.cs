using System;
using System.Collections.Generic;

namespace CanteenWebApi.Entities;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual MenuItem Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
