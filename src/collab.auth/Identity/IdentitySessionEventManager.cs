using System;
using System.Collections.Generic;

namespace Collab.Auth.Identity {
    /// <summary>
    /// Aggregates event handlers and manages raising events around the Identity Session lifecycle
    /// </summary>
    /// <typeparam name="TContext">The context that the session is maintained in (i.e. HttpContextBase)</typeparam>
    public interface IIdentitySessionEventManager<TContext> {
        /// <summary>
        /// Fire off all <see cref="IdentitySessionEvent.SigningIn"/> handlers 
        /// </summary>
        /// <param name="context">The current context</param>
        void OnSigningIn(TContext context);
        /// <summary>
        /// Fire off all <see cref="IdentitySessionEvent.SignedIn"/> handlers 
        /// </summary>
        /// <param name="context">The current context</param>
        void OnSignedIn(TContext context);
        /// <summary>
        /// Fire off all <see cref="IdentitySessionEvent.Restoring"/> handlers 
        /// </summary>
        /// <param name="context">The current context</param>
        void OnRestoring(TContext context);
        /// <summary>
        /// Fire off all <see cref="IdentitySessionEvent.Restored"/> handlers 
        /// </summary>
        /// <param name="context">The current context</param>
        void OnRestored(TContext context);
        /// <summary>
        /// Fire off all <see cref="IdentitySessionEvent.SigningOut"/> handlers 
        /// </summary>
        /// <param name="context">The current context</param>
        void OnSigningOut(TContext context);
        /// <summary>
        /// Fire off all <see cref="IdentitySessionEvent.SignedOut"/> handlers 
        /// </summary>
        /// <param name="context">The current context</param>
        void OnSignedOut(TContext context);
    }

    public class IdentitySessionEventManager<TContext> : IIdentitySessionEventManager<TContext> {
        private readonly IEnumerable<IIdentitySessionEventHandler<TContext>> _handlers;

        public IdentitySessionEventManager(IEnumerable<IIdentitySessionEventHandler<TContext>> handlers) {
            _handlers = handlers;
        }

        public void OnSigningIn(TContext context) {
            RaiseEvent(IdentitySessionEvent.SigningIn, context);
        }

        public void OnSignedIn(TContext context) {
            RaiseEvent(IdentitySessionEvent.SignedIn, context);
        }

        public void OnRestoring(TContext context) {
            RaiseEvent(IdentitySessionEvent.Restoring, context);
        }

        public void OnRestored(TContext context) {
            RaiseEvent(IdentitySessionEvent.Restored, context);
        }

        public void OnSigningOut(TContext context) {
            RaiseEvent(IdentitySessionEvent.SigningOut, context);
        }

        public void OnSignedOut(TContext context) {
            RaiseEvent(IdentitySessionEvent.SignedOut, context);
        }

        private void RaiseEvent(IdentitySessionEvent eventType, TContext context) {
            if (_handlers != null) {
                foreach (var handler in _handlers) {
                    try {
                        handler.OnSessionEvent(eventType, context);
                    }
                    catch (Exception ex) {
                        // TODO: Look at pulling handlers out of the set if they continually raise exceptions
                        // this may not happen much though.
                    }
                }
            }
        }
    }
}
