using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Collab.Auth {
    /// <summary>
    /// Wrapper for an Authentication result, which can be either successful or unsuccessful,
    /// successful results should contain an authenticated identity, unsuccessful results should specify errors.
    /// </summary>
    public class AuthenticationResult {
        /// <summary>
        /// An error AuthenticationResult indicating bad credentials.
        /// </summary>
        public static readonly AuthenticationResult InvalidCredentials = new AuthenticationResult("Invalid email address or password provided.");
        /// <summary>
        /// An error AuthenticationResult indicating that the underlying service is unavailable.
        /// </summary>
        public static readonly AuthenticationResult ServiceUnavailable = new AuthenticationResult("The authentication service is currently unavailable, please try again later.");
        /// <summary>
        /// An error AuthenticationResult indicating that the target masquerade user is invalid.
        /// </summary>
        public static readonly AuthenticationResult InvalidMasqueradeTarget = new AuthenticationResult("Invalid masquerade target email address.");
        /// <summary>
        /// An error AuthenticationResult indicating that the underlying session has expired (i.e. a remote session).
        /// </summary>
        public static readonly AuthenticationResult SessionExpired = new AuthenticationResult("Session expired.");
        /// <summary>
        /// An error AuthenticationResult indicating that the user does not have access to the current operating (i.e. masquerade).
        /// </summary>
        public static readonly AuthenticationResult AccessDenied = new AuthenticationResult("Access denied.");
        /// <summary>
        /// An neutral (unsuccessful) AuthenticationResult indicating that no action was taken (i.e. feature is turned off, etc.)
        /// </summary>
        public static readonly AuthenticationResult Empty = new AuthenticationResult(new string[] { });

        private readonly bool _success;
        private readonly ClaimsIdentity _identity;
        private readonly IList<string> _errors;

        public bool Successful { get { return _success; } }
        public ClaimsIdentity Identity { get { return _identity; } }
        public IList<string> Errors { get { return _errors; } }

        public AuthenticationResult(ClaimsIdentity identity) {
            _identity = identity ?? throw new ArgumentNullException("identity");
            _success = true;
        }

        public AuthenticationResult(IList<string> errors) {
            _errors = errors;
            _success = false;
        }

        public AuthenticationResult(string error)
            : this(new[] { error }) { }
    }
}
