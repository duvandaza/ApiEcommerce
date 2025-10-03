using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    public String? Name { get; set; }

    public String Username { get; set; } = string.Empty;

    public String? Password { get; set; }

    public String? Role { get; set; }
}
