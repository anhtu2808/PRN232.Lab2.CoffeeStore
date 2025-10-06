using System;
using System.Collections.Generic;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repositories.Entity;

public partial class Order
{
    public int OrderId { get; set; }

    public Guid UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public int? PaymentId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
