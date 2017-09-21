namespace Collab.Auth.Providers {
    /// <summary>
    /// An active authentication (username/password) provider
    /// </summary>
    public interface IActiveAuthenticationProvider {
        /// <summary>
        /// Attempt to authenticate a username/password combination against a particular environment.
        /// </summary>
        /// <param name="userName">The identifier of the user being authenticated</param>
        /// <param name="password">The password of the user being authenticated</param>
        /// <param name="environment">The target environment to authenticate against</param>
        /// <returns>An AuthenticationResult containing the result of the authentication request, see <see cref="AuthenticationResult"/>'s static members for predefined results</returns>
        AuthenticationResult Authenticate(string userName, string password, AuthenticationEnvironment environment);
    }
}
