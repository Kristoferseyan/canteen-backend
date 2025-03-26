using System;
using System.Collections.Generic;

namespace CanteenWebApi.Entities;

public partial class Category
{
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public Guid? ParentCategoryId { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual Category? ParentCategory { get; set; }
}
