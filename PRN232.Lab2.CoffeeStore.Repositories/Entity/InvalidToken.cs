using System;
using System.Collections.Generic;

namespace PRN232.Lab2.CoffeeStore.Repositories.Entity;

public partial class InvalidToken
{
    public int TokenId { get; set; }

    public string TokenValue { get; set; } = null!;

    public DateTime InvalidatedDate { get; set; }
}
