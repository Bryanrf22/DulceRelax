using Google.Cloud.Firestore;

namespace DulceRelax.API.Repositories
{
    public class DispositivoRepository(FirestoreDb db)
    {
        private CollectionReference Col => db.Collection("dispositivosAdmin");

        public Task GuardarAsync(string token) =>
            Col.Document(token).SetAsync(new Dictionary<string, object>
            {
                { "Actualizado", Timestamp.GetCurrentTimestamp() }
            });

        public Task EliminarAsync(string token) => Col.Document(token).DeleteAsync();

        public async Task<List<string>> GetTokensAsync() =>
            (await Col.GetSnapshotAsync()).Documents.Select(d => d.Id).ToList();
    }
}