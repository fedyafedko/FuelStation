using Microsoft.AspNetCore.Identity;

namespace FuelStation.DAL.Entities;

public class User : IdentityUser<Guid>
{
    public string RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}
