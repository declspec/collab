using System;
using System.Data;
using System.Threading;
using Microsoft.Data.Sqlite;

namespace Collab.Api.Database {
    public class SqliteConnectionProvider : IDbConnectionProvider {
        private readonly string _connectionString;
        private readonly ThreadLocal<InternalSqliteConnection> _connection;
        
        public SqliteConnectionProvider(string connectionString) {
            _connectionString = connectionString;

            var factory = new Func<InternalSqliteConnection>(() => {
                var conn = new InternalSqliteConnection(_connectionString);
                conn.Open();
                return conn;
            });

            _connection = new ThreadLocalDisposable<InternalSqliteConnection>(factory, conn => conn.Close(true));
        }

        public IDbConnection GetConnection() {
            return _connection.Value;
        }

        private class InternalSqliteConnection : SqliteConnection {
            public InternalSqliteConnection(string connectionString)
                : base(connectionString) { }

            public override void Close() {
                Close(false);
            }

            public void Close(bool disposing) {
                if (disposing)
                    base.Close();
            }
        }
    }

    // TODO: Find a better place for this to live. When it's used by something
    // else it should give a better idea. Could chuck it in Util but I hate Util.
    public class ThreadLocalDisposable<T> : ThreadLocal<T> {
        private readonly Action<T> _disposer;
        private bool _disposed;

        public ThreadLocalDisposable(Func<T> valueFactory, Action<T> disposer) : base(valueFactory) {
            _disposer = disposer;
            _disposed = false;
        }

        protected override void Dispose(bool disposing) {
            if (!_disposed && _disposer != null) {
                _disposed = true;
                _disposer(Value);
            }

            base.Dispose(disposing);
        }
    }
}
