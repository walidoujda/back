using System;
using System.Collections.Generic;

namespace back.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Firstname { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }
}
