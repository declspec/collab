using System.Data;
using Dapper;

namespace Collab.Api.Database.Migrations
{
    [Migration(20170921211700)]
    public class CreateUserTables : IMigration
    {
        public void Up(IDbConnection connection) {
            connection.Execute($@"CREATE TABLE { Tables.Users } (
                id              INTEGER PRIMARY KEY,
                first_name      TEXT NOT NULL,
                last_name       TEXT NOT NULL,
                email           TEXT NOT NULL UNIQUE,

                date_created    TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
                date_modified   TIMESTAMP
            );");
        }

        public void Down(IDbConnection connection) {
            connection.Execute($"DROP TABLE { Tables.Users }");
        }
}
}
