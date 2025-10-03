using System;

namespace ApiEcommerce.Models.Dtos.User;

public class UserRegisterDto
{
    public string? ID { get; set; }

    public required string Name { get; set; }

    public required string Username { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

}
