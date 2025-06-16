using System;
using System.Collections.Generic;

namespace back.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int Status { get; set; }

    public int UserId { get; set; }
}
