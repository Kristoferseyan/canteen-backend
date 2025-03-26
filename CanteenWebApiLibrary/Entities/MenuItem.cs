using System;
using System.Collections.Generic;

namespace CanteenWebApi.Entities;

public partial class MenuItem
{
    public Guid Id { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public Guid CategoryId { get; set; }

    public int Stock { get; set; }

    public DateTime? FeaturedStartTime { get; set; }

    public DateTime? FeaturedEndTime { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
