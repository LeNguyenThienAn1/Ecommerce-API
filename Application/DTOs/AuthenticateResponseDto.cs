using Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.DTOs
{
    public class AuthenticateResponseDto : BaseDto
    { 
    public string Name { get; set; }
    public UserType Role { get; set; }
    public string JwtToken { get; set; }

    [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }

    public AuthenticateResponseDto(UserEntity user, string jwtToken, string refreshToken)
    {
        Id = user.Id;
        Name = user.Name;
        Role = Role;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
    }
}
}
