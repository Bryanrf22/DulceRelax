using Google.Cloud.Firestore;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.API.Repositories
{
    public class UsuarioRepository
    {
        private readonly FirestoreDb _db;

        public UsuarioRepository(FirestoreDb db) => _db = db;

        public async Task<List<UsuarioDTO>> GetAllAsync()
        {
            var snapshot = await _db.Collection("usuarios").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<UsuarioDTO>()).ToList();
        }
    }
}
