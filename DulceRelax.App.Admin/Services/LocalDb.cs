using SQLite;

namespace DulceRelax.App.Admin.Services
{
    public class LocalDb
    {
        private readonly SemaphoreSlim _init = new(1, 1);
        private SQLiteAsyncConnection? _db;

        public async Task<SQLiteAsyncConnection> GetAsync()
        {
            if (_db is not null) return _db;

            await _init.WaitAsync();
            try
            {
                if (_db is null)
                {
                    var db = new SQLiteAsyncConnection(
                        Path.Combine(FileSystem.AppDataDirectory, "dulcerelax_admin.db3"));
                    await db.CreateTableAsync<CitaCache>();
                    await db.CreateTableAsync<ClienteCache>();
                    await db.CreateTableAsync<CuentaOffline>();
                    _db = db;
                }
                return _db;
            }
            finally { _init.Release(); }
        }
    }
}