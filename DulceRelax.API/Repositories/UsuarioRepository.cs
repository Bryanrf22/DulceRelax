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

        public async Task SetAsync(string id, UsuarioDTO usuario)
        {
            await _db.Collection("usuarios").Document(id).SetAsync(usuario);
        }

        public async Task<UsuarioDTO> GetByIdAsync(string id)
        {
            var doc = await _db.Collection("usuarios").Document(id).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<UsuarioDTO>() : null;
        }

        public async Task UpdateAsync(string id, ActualizarUsuarioDTO dto)
        {
            await _db.Collection("usuarios").Document(id).UpdateAsync(new Dictionary<string, object>
            {
                {"NombreCompleto", dto.NombreCompleto },
                {"NumTelefono", dto.NumTelefono },
                { "Direccion", dto.Direccion }
            });
        }

        public async Task DeleteAsync(string id)
        {
            await _db.Collection("usuarios").Document(id).DeleteAsync();
        }
    }
}
