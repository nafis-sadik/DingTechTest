using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace System.Core.Models
{
    public class RequestingUser
    {
        private readonly HttpContext _httpContext;
        private readonly ClaimsPrincipal _contextReference;

        internal RequestingUser(ClaimsPrincipal contextReference, HttpContext httpContext)
        {
            _httpContext = httpContext;
            _contextReference = contextReference;
        }

        public int UserId
        {
            get
            {
                if (!AllowedOrigins.Contains(RequestOrigin))
                    throw new ArgumentException($"{CommonConstants.HttpResponseMessages.InvalidToken} : Request origin is not allowed.");

                string? _userId = _contextReference.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.InvariantCultureIgnoreCase))?.Value;
                if (!string.IsNullOrWhiteSpace(_userId) && int.TryParse(_userId, out int userId))
                    return userId;
                else
                    throw new ArgumentException(CommonConstants.HttpResponseMessages.InvalidToken);
            }
        }

        public string RequestOrigin
        {
            get
            {
                string? requestingOrigin = _httpContext.Request.Headers["Origin"].ToString();


                if (!string.IsNullOrEmpty(requestingOrigin) && AllowedOrigins.Contains(requestingOrigin))
                    return requestingOrigin;

                else throw new ArgumentException($"{CommonConstants.HttpResponseMessages.InvalidToken} : Request origin is not allowed.");
            }

            private set { }
        }

        public string[] AllowedOrigins
        {
            get
            {
                string[] allowedOrigins = _contextReference.Claims.AsQueryable()
                    .Where(pair => pair.Type.Equals("allowed-origins"))
                    .Select(pair => pair.Value)
                    .ToArray();

                if (allowedOrigins != null && allowedOrigins.Length > 0) return allowedOrigins;

                throw new ArgumentException($"{CommonConstants.HttpResponseMessages.InvalidToken} : Claim with identifier 'jti' was not found.");
            }

            private set { }
        }

        public string TokenId
        {
            get
            {
                string? tokenId = _contextReference.Claims
                    .FirstOrDefault(x => x.Type.Equals("jti", StringComparison.InvariantCultureIgnoreCase))?.Value;

                if (!string.IsNullOrWhiteSpace(tokenId)) return tokenId;

                throw new ArgumentException($"{CommonConstants.HttpResponseMessages.InvalidToken} : Claim with identifier 'jti' was not found.");
            }

            private set { }
        }
    }
}