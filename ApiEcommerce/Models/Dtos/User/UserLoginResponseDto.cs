using System;

namespace ApiEcommerce.Models.Dtos.User;

public class UserLoginResponseDto
{
    public UserDataDto? User { get; set; }

    public String? Token { get; set; }

    public String? Message { get; set; }
}
