namespace Collab.Auth.Providers {
    public interface IPassiveAuthenticationProvider<TContext> {
        /// <summary>
        /// Attempt to passively authenticate a context for a specific environment
        /// </summary>
        /// <param name="context">The current context to authenticate</param>
        /// <param name="environment">The environment against which authentication should be attempted</param>
        /// <returns>
        /// A successful AuthenticationResult with an identity describing the user if the context's session was valid,
        /// see <see cref="AuthenticationResult" />'s static members for common error results
        /// </returns>
        AuthenticationResult Authenticate(TContext context, AuthenticationEnvironment environment);
    }
}
