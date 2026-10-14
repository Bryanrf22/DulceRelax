using System.Globalization;
using DulceRelax.Shared.DTOs;
using SQLite;

namespace DulceRelax.App.Admin.Services
{
    [Table("clientes")]
    public class ClienteCache
    {
        [PrimaryKey] public string Id { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string NumTelefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Direccion { get; set; } = "";
    }

    public interface IClienteCacheService
    {
        DateTime? UltimaSincronizacion { get; }
        Task GuardarAsync(IEnumerable<UsuarioDTO> clientes);
        Task<List<UsuarioDTO>> ObtenerTodosAsync();
    }

    public class ClienteCacheService(LocalDb local) : IClienteCacheService
    {
        private const string ClaveSync = "clientes_cache_sync";

        public DateTime? UltimaSincronizacion =>
            DateTime.TryParse(Preferences.Get(ClaveSync, ""), CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var f) ? f : null;

        public async Task GuardarAsync(IEnumerable<UsuarioDTO> clientes)
        {
            var filas = clientes.Select(c => new ClienteCache
            {
                Id = c.Id,
                NombreCompleto = c.NombreCompleto ?? "",
                NumTelefono = c.NumTelefono ?? "",
                Correo = c.Correo ?? "",
                Direccion = c.Direccion ?? ""
            }).ToList();

            var db = await local.GetAsync();
            await db.RunInTransactionAsync(conn =>
            {
                conn.DeleteAll<ClienteCache>();
                conn.InsertAll(filas);
            });

            Preferences.Set(ClaveSync, DateTime.UtcNow.ToString("o"));
        }

        public async Task<List<UsuarioDTO>> ObtenerTodosAsync()
        {
            var db = await local.GetAsync();
            var filas = await db.Table<ClienteCache>().ToListAsync();

            return filas.Select(f => new UsuarioDTO
            {
                Id = f.Id,
                NombreCompleto = f.NombreCompleto,
                NumTelefono = f.NumTelefono,
                Correo = f.Correo,
                Direccion = f.Direccion
            }).ToList();
        }
    }
}