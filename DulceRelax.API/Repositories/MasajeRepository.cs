using Google.Cloud.Firestore;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.API.Repositories
{
    public class MasajeRepository
    {
        private readonly FirestoreDb _db;

        public MasajeRepository(FirestoreDb db) => _db = db;

        public async Task<List<MasajeDTO>> GetAllAsync()
        {
            var snapshot = await _db.Collection("masajes").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<MasajeDTO>()).ToList();
        }

        public async Task<string> CreateAsync(MasajeDTO masaje)
        {
            var docRef = _db.Collection("masajes").Document();
            masaje.Id = docRef.Id;
            await docRef.SetAsync(masaje);
            return docRef.Id;
        }

        public async Task<MasajeDTO> GetById(string id)
        {
            var doc = await _db.Collection("masajes").Document(id).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<MasajeDTO>() : null;
        }

        public async Task UpdateAsync(string id, ActualizarMasajeDTO dto)
        {
            await _db.Collection("masajes").Document(id).UpdateAsync(new Dictionary<string, object>
            {
                { "Nombre", dto.Nombre },
                { "Descripcion", dto.Descripcion },
                { "Disponible", dto.Disponible },
                { "Precio", dto.Precio }
            });
        }

        public async Task DeleteAsync(string id)
        {
            await _db.Collection("masajes").Document(id).DeleteAsync();
        }



    }
}
