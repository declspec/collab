using System.Collections.Generic;
using System.Security.Claims;

namespace Collab.Auth.Extensions {
    /// <summary>
    /// Generic helper methods for dealing with claims
    /// </summary>
    public static class SecurityExtensions {
        /// <summary>
        /// Retrieves all role claims from all identities in the provided principal
        /// </summary>
        /// <param name="principal">Principal to extract roles from</param>
        /// <returns>An enumerable of roles contained in the principal</returns>
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal) {
            foreach (var identity in principal.Identities) {
                foreach (var claim in identity.FindAll(identity.RoleClaimType))
                    yield return claim.Value;
            }
        }

        /// <summary>
        /// Retrieves all role claims in the provided identity
        /// </summary>
        /// <param name="identity">Identity to extract roles from</param>
        /// <returns>An enumerable of roles contained in the principal</returns>
        public static IEnumerable<string> GetRoles(this ClaimsIdentity identity) {
            foreach (var claim in identity.FindAll(identity.RoleClaimType))
                yield return claim.Value;
        }

        /// <summary>
        /// Retrieves the value of the first claim with a matching type.
        /// </summary>
        /// <param name="principal">Principal to extract the claim from</param>
        /// <param name="claimType">Claim type to look up</param>
        /// <returns></returns>
        public static string GetClaim(this ClaimsPrincipal principal, string claimType) {
            foreach (var claim in principal.Claims) {
                if (claim.Type == claimType)
                    return claim.Value;
            }
            return null;
        }

        /// <summary>
        /// Retrieves the value of the first claim with a matching type.
        /// </summary>
        /// <param name="identity">Principal to extract the claim from</param>
        /// <param name="claimType">Claim type to look up</param>
        /// <returns></returns>
        public static string GetClaim(this ClaimsIdentity identity, string claimType) {
            foreach (var claim in identity.Claims) {
                if (claim.Type == claimType)
                    return claim.Value;
            }
            return null;
        }
    }
}
