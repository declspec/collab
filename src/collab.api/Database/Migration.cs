using System;
using System.Data;

namespace Collab.Api.Database {

    [AttributeUsage(AttributeTargets.Class)]
    public class MigrationAttribute : Attribute {
        public long Version { get; }
        public string Title { get; }

        public MigrationAttribute(long version)
            : this(version, null) { }

        public MigrationAttribute(long version, string title) {
            Version = version;
            Title = title;
        }
    }

    public interface IMigration {
        void Up(IDbConnection connection);
        void Down(IDbConnection connection);
    }
}