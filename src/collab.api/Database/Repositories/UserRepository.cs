using System.Collections.Generic;
using System.Linq;
using Collab.Api.Models;
using Collab.Api.Database.Entities;
using Dapper;

namespace Collab.Api.Database.Repositories {
    public interface IUserRepository {
        User GetById(int id);
        User GetByEmail(string email);
    }

    public class UserRepository : IUserRepository {
        private const string UserFieldList = "{0}.id, {0}.first_name, {0}.last_name, {0}.email, {0}.password encrypted_password, {0}.current_token, {0}.date_created, {0}.date_modified";

        private static readonly string Select = GetSelectQuery();

        private readonly IDbConnectionProvider _connectionProvider;

        public UserRepository(IDbConnectionProvider connectionProvider) {
            _connectionProvider = connectionProvider;
        }

        public User GetById(int id) {
            return ToModel(GetUsersWhere("{0}.id = @Id", new { Id = id }).SingleOrDefault());
        }

        public User GetByEmail(string email) {
            return ToModel(GetUsersWhere("{0}.email = @Email", new { Email = email.ToLower() }).SingleOrDefault());
        }
        
        private IEnumerable<UserEntity> GetUsersWhere(string clauseFormat, object param = null) {
            var clause = string.Format(clauseFormat, Tables.Users);
            using (var connection = _connectionProvider.GetConnection())
                return connection.Query<UserEntity>(string.Format("{0} WHERE {1}", Select, clause), param);
        }

        private static string GetSelectQuery() {
            var userFields = string.Format(UserFieldList, Tables.Users);
            return string.Format("SELECT {0} FROM {1}", userFields, Tables.Users);
        }

        // 
        // Mapping
        //
        private static User ToModel(UserEntity entity) {
            if (entity == null)
                return null;

            return new User() {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                DateCreated = entity.DateCreated,
                DateModified = entity.DateModified
            };
        }
    }
}
