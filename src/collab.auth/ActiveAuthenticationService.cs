using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Collab.Auth.Providers;
using Collab.Auth.Extensions;

namespace Collab.Auth {
    public interface IActiveAuthenticationService {
        /// <summary>
        /// Authenticate a user by their credentials against the current server environment
        /// </summary>
        /// <param name="userName">The identifier of the user being authenticated</param>
        /// <param name="password">The password of the user being authenticated</param>
        /// <returns>
        /// An AuthenticationResult containing all the distinct errors from all registered providers if none are successful
        /// An AuthenticationResult containing the authenticated identity if one of the providers was successful
        /// </returns>
        /// <returns>An AuthenticationResult containing either an authenticated identity of the user, or a set of errors</returns>
        AuthenticationResult Authenticate(string userName, string password);

        /// <summary>
        /// Authenticate a user by their credentials
        /// </summary>
        /// <param name="userName">The identifier of the user being authenticated</param>
        /// <param name="password">The password of the user being authenticated</param>
        /// <param name="environment">The target environment to authenticate against</param>
        /// <returns>
        /// An AuthenticationResult containing all the distinct errors from all registered providers if none are successful
        /// An AuthenticationResult containing the authenticated identity if one of the providers was successful
        /// </returns>
        /// <returns>An AuthenticationResult containing either an authenticated identity of the user, or a set of errors.</returns>
        AuthenticationResult Authenticate(string userName, string password, AuthenticationEnvironment environment);

        /// <summary>Attempt to retrieve a masqueraded identity for a target user name</summary>
        /// <param name="currentPrincipal">The principal attempting to masquerade</param>
        /// <param name="targetUserName">The name of the user to obtain a masqueraded identity from</param>
        /// <returns>
        /// AuthenticationResult.Empty if no Masquerade providers have been registered, or if all return AuthentiationResult.Empty
        /// AuthenticationResult.InvalidMasqueradeTarget if the target user name is invalid
        /// An AuthenticationResult containing all the distinct errors from all registered providers if none are successful
        /// An AuthenticationResult containing the masqueraded identity if one of the providers was successful
        /// </returns>
        AuthenticationResult Masquerade(ClaimsPrincipal currentPrincipal, string targetUserName);
    }

    public class ActiveAuthenticationService : IActiveAuthenticationService {
        private readonly AuthenticationEnvironment _environment;
        private readonly IEnumerable<IActiveAuthenticationProvider> _authenticationProviders;
        private readonly IEnumerable<IMasqueradeProvider> _masqueradeProviders;

        private readonly IList<string> _masqueradeRoles;
        private readonly IList<string> _restrictedMasqueradeRoles;

        public ActiveAuthenticationService(AuthenticationEnvironment environment, IEnumerable<IActiveAuthenticationProvider> authenticationProviders, IEnumerable<IMasqueradeProvider> masqueradeProviders, IList<string> masqueradeRoles = null, IList<string> restrictedMasqueradeRoles = null) {
            _environment = environment;
            _authenticationProviders = authenticationProviders;
            _masqueradeProviders = masqueradeProviders;

            _masqueradeRoles = masqueradeRoles;
            _restrictedMasqueradeRoles = restrictedMasqueradeRoles;
        }

        public AuthenticationResult Authenticate(string userName, string password) {
            return Authenticate(userName, password, _environment);
        }

        public AuthenticationResult Authenticate(string userName, string password, AuthenticationEnvironment environment) {
            return Identify(_authenticationProviders, provider => provider.Authenticate(userName, password, environment));
        }

        public AuthenticationResult Masquerade(ClaimsPrincipal principal, string targetUserName) {
            if (principal == null)
                throw new ArgumentNullException("principal");

            if (_masqueradeProviders == null || !_masqueradeProviders.Any())
                return new AuthenticationResult("No masquerade providers configured in the application.");

            if (!HasMasqueradePermission(principal))
                return AuthenticationResult.AccessDenied;

            if (string.IsNullOrEmpty(targetUserName))
                return AuthenticationResult.InvalidMasqueradeTarget;

            var result = Identify(_masqueradeProviders, provider => provider.Masquerade(principal, targetUserName));

            if (result.Successful && !IsAccessibleMasqueradeTarget(principal, result.Identity))
                result = new AuthenticationResult("Insufficient privileges to masquerade as target user.");

            return result;
        }

        // Run through a list of providers and find the first successful one given a specific resolver
        private static AuthenticationResult Identify<TProvider>(IEnumerable<TProvider> providers, Func<TProvider, AuthenticationResult> resultResolver) {
            // If no providers are supplied, return an empty unsuccessful result.
            if (providers == null)
                return AuthenticationResult.Empty;

            var failureResults = new HashSet<AuthenticationResult>();

            foreach (var provider in providers) {
                var result = resultResolver(provider);

                if (result == null)
                    continue;

                if (result.Successful)
                    return result;

                failureResults.Add(result);
            }

            switch (failureResults.Count) {
                case 0: return AuthenticationResult.Empty;
                case 1: return failureResults.First();
                default:
                    // Aggregate all the error messages into a new AuthenticationResult
                    var errors = failureResults.SelectMany(r => r.Errors).Distinct().ToList();
                    return errors.Count == 0 ? AuthenticationResult.Empty : new AuthenticationResult(errors);
            }
        }

        private bool HasMasqueradePermission(ClaimsPrincipal principal) {
            // Has permission when masqueradeRoles is explicitly set to empty, 
            // or if the authentication service roles contains an approved role, 
            // or any role providers return an approved role for the user
            var principalRoles = principal.GetRoles();
            return _masqueradeRoles != null && (_masqueradeRoles.Count == 0 || _masqueradeRoles.Any(role => principalRoles.Contains(role, StringComparer.OrdinalIgnoreCase)));
        }

        private bool IsAccessibleMasqueradeTarget(ClaimsPrincipal principal, ClaimsIdentity targetIdentity) {
            if (_restrictedMasqueradeRoles == null)
                return true;

            // Ensure that the real user has similar permission levels to the masquerade target.
            // Stops users from elevating their own access by masquerading as a target with more permissions.
            return !targetIdentity.GetRoles().Except(principal.GetRoles(), StringComparer.OrdinalIgnoreCase)
                .Any(role => _restrictedMasqueradeRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
        }
    }
}
