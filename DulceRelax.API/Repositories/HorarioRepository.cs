using DulceRelax.Shared;
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
            return CitaRules.EstaDentroDelHorarioUtc(utc, minutos, horario, _zona);
        }
    }
}