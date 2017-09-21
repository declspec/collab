using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Collab.Auth.Providers {
    /// <summary>
    /// Claim provider that transforms the results of IRoleProviders
    /// into role claims
    /// </summary>
    public class RoleClaimsProvider : IClaimsProvider {
        private readonly IEnumerable<IRoleProvider> _roleProviders;

        public RoleClaimsProvider(IEnumerable<IRoleProvider> roleProviders) {
            _roleProviders = roleProviders;
        }

        public IEnumerable<Claim> GetClaims(ClaimsIdentity identity) {
            if (identity == null)
                throw new ArgumentNullException("identity");

            var claimType = string.IsNullOrEmpty(identity.RoleClaimType) ? ClaimsIdentity.DefaultRoleClaimType : identity.RoleClaimType;

            return _roleProviders.SelectMany(provider => {
                var providerId = provider.GetType().Name;

                try {
                    return provider.GetRolesForUser(identity.Name).Select(role => new Claim(claimType, role, ClaimValueTypes.String, providerId));
                }
                catch (Exception ex) {
                    throw new Exception(string.Format("Role provider {0} failed for {1}", providerId, identity.Name), ex);
                }
            });
        }
    }
}
