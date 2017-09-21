namespace Collab.Auth.Identity
{
    /// <summary>
    /// Events that occur during an Identity Session's lifecycle
    /// </summary>
    public enum IdentitySessionEvent
    {
        /// <summary>
        /// Raised when SignIn is called, but before doing any work.
        /// </summary>
        SigningIn,
        /// <summary>
        /// Raised when SignIn is called, after all the work is done.
        /// </summary>
        SignedIn,
        /// <summary>
        /// Raised when TryRestore is called, but before doing any work.
        /// </summary>
        Restoring,
        /// <summary>
        /// Raised when an Identity Session was successfully restored, if no such session exists this will not fire.
        /// </summary>
        Restored,
        /// <summary>
        /// Raised when SignOut is called, but before doing any work.
        /// </summary>
        SigningOut,
        /// <summary>
        /// Raised when SignOut is called, after all the work is done.
        /// </summary>
        SignedOut
    }
}
