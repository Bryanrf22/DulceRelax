using System.Globalization;
using DulceRelax.Shared.DTOs;
using SQLite;

namespace DulceRelax.App.Admin.Services
{
    [Table("citas")]
    public class CitaCache
    {
        [PrimaryKey] public string Id { get; set; } = "";
        public string UsuarioNombre { get; set; } = "";
        public string UsuarioTelefono { get; set; } = "";
        public string DireccionExacta { get; set; } = "";
        public string? DetalleDireccion { get; set; }
        public string MasajeNombre { get; set; } = "";
        public DateTime FechaHoraUtc { get; set; }
    }

    public interface ICitaCacheService
    {
        DateTime? UltimaSincronizacion { get; }
        bool CubreDia(DateTime dia);
        Task GuardarAsync(IEnumerable<RespuestaCitaDTO> citas);
        Task<List<RespuestaCitaDTO>> ObtenerDiaAsync(DateTime dia);
    }

    public class CitaCacheService(LocalDb local) : ICitaCacheService
    {
        private const string ClaveSync = "citas_cache_sync";
        
        public DateTime? UltimaSincronizacion =>
            DateTime.TryParse(Preferences.Get(ClaveSync, ""), CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var f) ? f : null;

        public bool CubreDia(DateTime dia)
        {
            var sync = UltimaSincronizacion?.ToLocalTime().Date;
            if (sync is null) return false;

            var d = dia.Date;
            return d >= DateTime.Today && d <= DateTime.Today.AddDays(1)
                && d >= sync && d <= sync.Value.AddDays(1);
        }

        public async Task GuardarAsync(IEnumerable<RespuestaCitaDTO> citas)
        {
            var filas = citas
                .Where(c => c.Estado == EstadoCita.Confirmada)
                .Select(c => new CitaCache
                {
                    Id = c.Id,
                    UsuarioNombre = c.UsuarioNombre,
                    UsuarioTelefono = c.UsuarioTelefono,
                    DireccionExacta = c.DireccionExacta,
                    DetalleDireccion = c.DetalleDireccion,
                    MasajeNombre = c.MasajeNombre,
                    FechaHoraUtc = c.FechaHora.ToUniversalTime()
                })
                .ToList();

            var db = await local.GetAsync();
            await db.RunInTransactionAsync(conn =>
            {
                conn.DeleteAll<CitaCache>();
                conn.InsertAll(filas);
            });

            Preferences.Set(ClaveSync, DateTime.UtcNow.ToString("o"));
        }

        public async Task<List<RespuestaCitaDTO>> ObtenerDiaAsync(DateTime dia)
        {
            var desde = new DateTimeOffset(dia.Date).UtcDateTime;
            var hasta = desde.AddDays(1);

            var db = await local.GetAsync();
            var filas = await db.Table<CitaCache>()
                .Where(c => c.FechaHoraUtc >= desde && c.FechaHoraUtc < hasta)
                .OrderBy(c => c.FechaHoraUtc)
                .ToListAsync();

            return filas.Select(f => new RespuestaCitaDTO
            {
                Id = f.Id,
                UsuarioNombre = f.UsuarioNombre,
                UsuarioTelefono = f.UsuarioTelefono,
                DireccionExacta = f.DireccionExacta,
                DetalleDireccion = f.DetalleDireccion,
                MasajeNombre = f.MasajeNombre,
                FechaHora = DateTime.SpecifyKind(f.FechaHoraUtc, DateTimeKind.Utc),
                Estado = EstadoCita.Confirmada
            }).ToList();
        }

    }
}