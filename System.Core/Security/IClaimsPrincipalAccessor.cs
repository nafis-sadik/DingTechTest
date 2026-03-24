using System.Security.Claims;

namespace System.Core.Security
{
    public interface IClaimsPrincipalAccessor
    {
        ClaimsPrincipal GetCurrentPrincipal();
    }
}
