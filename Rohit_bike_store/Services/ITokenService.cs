using Microsoft.AspNetCore.Identity;

namespace Rohit_bike_store.Services;

public interface ITokenService
{
    string CreateJWTToken(IdentityUser user, List<string> roles);
}