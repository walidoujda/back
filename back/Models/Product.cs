using System;
using System.Collections.Generic;

namespace back.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public float? Price { get; set; }

    public int? Quantity { get; set; }

    public string? InternalReference { get; set; }

    public string? ShellId { get; set; }

    public int? Rating { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }
}
