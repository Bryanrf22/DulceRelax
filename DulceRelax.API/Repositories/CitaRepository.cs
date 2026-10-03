using DulceRelax.Shared.DTOs;
using Google.Cloud.Firestore;

namespace DulceRelax.API.Repositories
{
    public enum ResultadoCita { Ok, NoEncontrada, HorarioOcupado }

    public class CitaRepository
    {
        private static readonly TimeSpan Duracion = TimeSpan.FromHours(1);
        private readonly FirestoreDb _db;

        public CitaRepository(FirestoreDb db) => _db = db;

        private CollectionReference Citas => _db.Collection("citas");

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

        public async Task<string?> CrearAsync(RegistrarCitaDTO dto)
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
                FechaHora = Timestamp.FromDateTime(inicio),
                Estado = EstadoCita.Pendiente
            };

            var creada = await _db.RunTransactionAsync(async tx =>
            {
                if (await HayConflictoAsync(tx, inicio, null)) return false;
                tx.Create(docRef, cita);
                return true;
            });

            return creada ? docRef.Id : null;
        }

        public async Task<ResultadoCita> ActualizarAsync(string id, ActualizarCitaDTO dto)
        {
            var docRef = Citas.Document(id);
            var inicio = dto.FechaHora.ToUniversalTime();

            return await _db.RunTransactionAsync(async tx =>
            {
                var doc = await tx.GetSnapshotAsync(docRef);
                if (!doc.Exists) return ResultadoCita.NoEncontrada;

                if (dto.Estado != EstadoCita.Cancelada && await HayConflictoAsync(tx, inicio, id))
                    return ResultadoCita.HorarioOcupado;

                tx.Update(docRef, new Dictionary<string, object>
                {
                    { "DireccionExacta", dto.DireccionExacta },
                    { "DetalleDireccion", dto.DetalleDireccion ?? "" },
                    { "MasajeId", dto.MasajeId },
                    { "MasajeNombre", dto.MasajeNombre },
                    { "FechaHora", Timestamp.FromDateTime(inicio) },
                    { "Estado", dto.Estado }
                });
                return ResultadoCita.Ok;
            });
        }

        public async Task<bool> CambiarEstadoAsync(string id, EstadoCita estado)
        {
            var docRef = Citas.Document(id);
            var doc = await docRef.GetSnapshotAsync();
            if (!doc.Exists) return false;
            await docRef.UpdateAsync("Estado", estado);
            return true;
        }

        private async Task<bool> HayConflictoAsync(Transaction tx, DateTime inicio, string? excluirId)
        {
            var query = Citas
                .WhereGreaterThan("FechaHora", Timestamp.FromDateTime(inicio - Duracion))
                .WhereLessThan("FechaHora", Timestamp.FromDateTime(inicio + Duracion));

            var snap = await tx.GetSnapshotAsync(query);
            return snap.Documents.Any(d =>
                d.Id != excluirId &&
                d.ConvertTo<CitaDTO>().Estado != EstadoCita.Cancelada);
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
    }
}