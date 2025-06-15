using System;
using System.Collections.Generic;

namespace back.Models;

public partial class Orderitem
{
    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }
}
