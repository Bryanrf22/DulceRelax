using DulceRelax.Shared.DTOs;
using Google.Cloud.Firestore;

namespace DulceRelax.API.Repositories
{
    public enum ResultadoCita { Ok, NoEncontrada, HorarioOcupado }

    public class CitaRepository
    {
        private static readonly TimeSpan Descanso = TimeSpan.FromHours(1);
        private static readonly TimeSpan DuracionPorDefecto = TimeSpan.FromHours(1);
        private static readonly TimeSpan BloqueoMaximo = TimeSpan.FromHours(5);

        private readonly FirestoreDb _db;

        public CitaRepository(FirestoreDb db) => _db = db;

        private CollectionReference Citas => _db.Collection("citas");

        private static TimeSpan Bloque(int minutos) =>
            (minutos > 0 ? TimeSpan.FromMinutes(minutos) : DuracionPorDefecto) + Descanso;

        private static bool Bloquea(CitaDTO c) => c.Estado == EstadoCita.Confirmada;

        public async Task<RespuestaCitaDTO?> GetByIdAsync(string id)
        {
            var doc = await Citas.Document(id).GetSnapshotAsync();
            return doc.Exists ? ToRespuesta(doc.ConvertTo<CitaDTO>()) : null;
        }

        public async Task<List<RespuestaCitaDTO>> GetByUsuarioAsync(string usuarioId)
        {
            var snap = await Citas.WhereEqualTo("UsuarioId", usuarioId).GetSnapshotAsync();
            return snap.Documents
                .Select(d => ToRespuesta(d.ConvertTo<CitaDTO>()))
                .OrderBy(c => c.FechaHora)
                .ToList();
        }

        public async Task<List<RespuestaCitaDTO>> GetByFechaAsync(DateTime fecha)
        {
            var inicio = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
            var snap = await Citas
                .WhereGreaterThanOrEqualTo("FechaHora", Timestamp.FromDateTime(inicio))
                .WhereLessThan("FechaHora", Timestamp.FromDateTime(inicio.AddDays(1)))
                .GetSnapshotAsync();
            return snap.Documents
                .Select(d => ToRespuesta(d.ConvertTo<CitaDTO>()))
                .OrderBy(c => c.FechaHora)
                .ToList();
        }

        public async Task<List<RespuestaCitaDTO>> GetConfirmadasPorRangoAsync(DateTime desdeUtc, DateTime hastaUtc)
        {
            var snap = await Citas
                .WhereGreaterThanOrEqualTo("FechaHora", Timestamp.FromDateTime(desdeUtc))
                .WhereLessThan("FechaHora", Timestamp.FromDateTime(hastaUtc))
                .GetSnapshotAsync();

            return snap.Documents
                .Select(d => ToRespuesta(d.ConvertTo<CitaDTO>()))
                .Where(c => c.Estado == EstadoCita.Confirmada)
                .OrderBy(c => c.FechaHora)
                .ToList();
        }

        public async Task<List<(DateTime Inicio, DateTime Fin)>> GetBloqueosAsync(DateTime desdeUtc, DateTime hastaUtc)
        {
            var snap = await Citas
                .WhereGreaterThan("FechaHora", Timestamp.FromDateTime(desdeUtc - BloqueoMaximo))
                .WhereLessThan("FechaHora", Timestamp.FromDateTime(hastaUtc))
                .GetSnapshotAsync();

            return snap.Documents
                .Select(d => d.ConvertTo<CitaDTO>())
                .Where(Bloquea)
                .Select(c =>
                {
                    var ini = c.FechaHora.ToDateTime();
                    return (Inicio: ini, Fin: ini + Bloque(c.DuracionMinutos));
                })
                .Where(b => b.Fin > desdeUtc)
                .OrderBy(b => b.Inicio)
                .ToList();
        }

        public async Task<string?> CrearAsync(RegistrarCitaDTO dto, int duracionMinutos)
        {
            var inicio = dto.FechaHora.ToUniversalTime();
            var docRef = Citas.Document();
            var cita = new CitaDTO
            {
                UsuarioId = dto.UsuarioId,
                UsuarioNombre = dto.UsuarioNombre,
                UsuarioTelefono = dto.UsuarioTelefono,
                DireccionExacta = dto.DireccionExacta,
                DetalleDireccion = dto.DetalleDireccion,
                MasajeId = dto.MasajeId,
                MasajeNombre = dto.MasajeNombre,
                DuracionMinutos = duracionMinutos,
                FechaHora = Timestamp.FromDateTime(inicio),
                Estado = EstadoCita.Pendiente
            };

            var creada = await _db.RunTransactionAsync(async tx =>
            {
                if (await HayConflictoAsync(tx, inicio, duracionMinutos, null)) return false;
                tx.Create(docRef, cita);
                return true;
            });

            return creada ? docRef.Id : null;
        }

        public async Task<ResultadoCita> ActualizarAsync(string id, ActualizarCitaDTO dto, int duracionMinutos)
        {
            var docRef = Citas.Document(id);
            var inicio = dto.FechaHora.ToUniversalTime();

            return await _db.RunTransactionAsync(async tx =>
            {
                var doc = await tx.GetSnapshotAsync(docRef);
                if (!doc.Exists) return ResultadoCita.NoEncontrada;

                if (dto.Estado != EstadoCita.Cancelada &&
                    await HayConflictoAsync(tx, inicio, duracionMinutos, id))
                    return ResultadoCita.HorarioOcupado;

                tx.Update(docRef, new Dictionary<string, object>
                {
                    { "DireccionExacta", dto.DireccionExacta },
                    { "DetalleDireccion", dto.DetalleDireccion ?? "" },
                    { "MasajeId", dto.MasajeId },
                    { "MasajeNombre", dto.MasajeNombre },
                    { "DuracionMinutos", duracionMinutos },
                    { "FechaHora", Timestamp.FromDateTime(inicio) },
                    { "Estado", dto.Estado }
                });
                return ResultadoCita.Ok;
            });
        }

        public async Task<ResultadoCita> CambiarEstadoAsync(string id, EstadoCita estado)
        {
            var docRef = Citas.Document(id);

            return await _db.RunTransactionAsync(async tx =>
            {
                var doc = await tx.GetSnapshotAsync(docRef);
                if (!doc.Exists) return ResultadoCita.NoEncontrada;

                if (estado == EstadoCita.Confirmada)
                {
                    var c = doc.ConvertTo<CitaDTO>();
                    if (await HayConflictoAsync(tx, c.FechaHora.ToDateTime(), c.DuracionMinutos, id))
                        return ResultadoCita.HorarioOcupado;
                }

                tx.Update(docRef, new Dictionary<string, object> { { "Estado", estado } });
                return ResultadoCita.Ok;
            });
        }

        private async Task<bool> HayConflictoAsync(Transaction tx, DateTime inicio, int duracionMinutos, string? excluirId)
        {
            var finBloque = inicio + Bloque(duracionMinutos);
            var query = Citas
                .WhereGreaterThan("FechaHora", Timestamp.FromDateTime(inicio - BloqueoMaximo))
                .WhereLessThan("FechaHora", Timestamp.FromDateTime(finBloque));

            var snap = await tx.GetSnapshotAsync(query);
            return snap.Documents.Any(d =>
            {
                if (d.Id == excluirId) return false;

                var c = d.ConvertTo<CitaDTO>();
                if (!Bloquea(c)) return false;

                var ini = c.FechaHora.ToDateTime();
                return ini < finBloque && ini + Bloque(c.DuracionMinutos) > inicio;
            });
        }

        private static RespuestaCitaDTO ToRespuesta(CitaDTO c) => new()
        {
            Id = c.Id,
            UsuarioId = c.UsuarioId,
            UsuarioNombre = c.UsuarioNombre,
            UsuarioTelefono = c.UsuarioTelefono,
            DireccionExacta = c.DireccionExacta,
            DetalleDireccion = c.DetalleDireccion,
            MasajeId = c.MasajeId,
            MasajeNombre = c.MasajeNombre,
            FechaHora = c.FechaHora.ToDateTime(),
            Estado = c.Estado
        };

        public async Task<List<RespuestaCitaDTO>> GetByRangoAsync(DateTime desdeUtc, DateTime hastaUtc)
        {
            var snap = await Citas
                .WhereGreaterThanOrEqualTo("FechaHora", Timestamp.FromDateTime(desdeUtc))
                .WhereLessThan("FechaHora", Timestamp.FromDateTime(hastaUtc))
                .GetSnapshotAsync();

            return snap.Documents
                .Select(d => ToRespuesta(d.ConvertTo<CitaDTO>()))
                .OrderBy(c => c.FechaHora)
                .ToList();
        }

        public async Task<List<RespuestaCitaDTO>> GetPendientesAsync()
        {
            var snap = await Citas
                .WhereGreaterThanOrEqualTo("FechaHora", Timestamp.FromDateTime(DateTime.UtcNow))
                .GetSnapshotAsync();

            return snap.Documents
                .Select(d => ToRespuesta(d.ConvertTo<CitaDTO>()))
                .Where(c => c.Estado == EstadoCita.Pendiente)
                .OrderBy(c => c.FechaHora)
                .ToList();
        }
    }
}