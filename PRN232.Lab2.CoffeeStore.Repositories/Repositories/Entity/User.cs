using System;
using System.Collections.Generic;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repositories.Entity;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
