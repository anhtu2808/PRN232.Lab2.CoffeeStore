using System;
using System.Collections.Generic;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repositories.Entity;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
