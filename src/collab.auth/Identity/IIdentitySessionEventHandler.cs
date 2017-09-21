namespace Collab.Auth.Identity
{
    /// <summary>
    /// Handler for Identity Session lifecycle events in a specific context
    /// <seealso cref="IIdentitySessionEventManager{TContext}"/> <seealso cref="IIdentitySessionManager"/>
    /// </summary>
    /// <typeparam name="TContext">The context that the session is maintained in (i.e. HttpContextBase)</typeparam>
    /// <remarks>
    /// All IIdentitySessionEventHandler instances of a specific generic type are dependency injected into
    /// the parent <see cref="IIdentitySessionEventManager{TContext}"/>, this allows a class to hook into
    /// the identity session lifecycle just by implementing the interface (and being registered in the DI container).
    /// This allows a class to clean up any state it's created (i.e. remote sessions) as the lifecycle changes.
    /// </remarks>
    public interface IIdentitySessionEventHandler<TContext>
    {
        /// <summary>
        /// Invoked whenever an <see cref="IdentitySessionEvent"/> is raised 
        /// </summary>
        /// <param name="eventType">The type of event that occurred</param>
        /// <param name="context">The current context the event was raised in</param>
        void OnSessionEvent(IdentitySessionEvent eventType, TContext context);
    }
}
