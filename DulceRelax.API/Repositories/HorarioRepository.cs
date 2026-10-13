using DulceRelax.Shared.DTOs;
using Google.Cloud.Firestore;

namespace DulceRelax.API.Repositories
{
    public class HorarioRepository
    {
        private readonly FirestoreDb _db;
        private readonly TimeZoneInfo _zona;

        public HorarioRepository(FirestoreDb db, IConfiguration config)
        {
            _db = db;
            _zona = TimeZoneInfo.FindSystemTimeZoneById(
                config["Negocio:ZonaHoraria"] ?? "America/Managua");
        }

        private DocumentReference Doc => _db.Collection("configuracion").Document("horario");

        public async Task<HorarioAtencionDTO?> GetAsync()
        {
            var snap = await Doc.GetSnapshotAsync();
            return snap.Exists ? snap.ConvertTo<HorarioAtencionDTO>() : null;
        }

        public Task SetAsync(HorarioAtencionDTO dto) => Doc.SetAsync(dto);

        public async Task<bool> EstaDentroAsync(DateTime utc, int minutos)
        {
            var horario = await GetAsync();
            if (horario?.Dias is null || horario.Dias.Count == 0) return true;

            var inicio = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(utc, DateTimeKind.Utc), _zona);
            var fin = inicio.AddMinutes(minutos);

            if (!horario.Dias.TryGetValue(inicio.DayOfWeek.ToString(), out var dia) || !dia.Activo)
                return false;

            if (!TimeSpan.TryParse(dia.HoraInicio, out var abre) ||
                !TimeSpan.TryParse(dia.HoraFin, out var cierra))
                return false;

            return fin.Date == inicio.Date
                && inicio.TimeOfDay >= abre
                && fin.TimeOfDay <= cierra;
        }
    }
}