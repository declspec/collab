using System.Collections.Generic;

namespace Collab.Auth.Providers {
    ///<summary>
    /// Provides roles for users
    ///</summary>
    public interface IRoleProvider {
        /// <summary>
        /// Provides roles for the given user
        /// </summary>
        /// <param name="userName">Username of the user to provide roles for</param>
        /// <returns>A list of the roles for the user</returns>
        IEnumerable<string> GetRolesForUser(string userName);
    }
}
