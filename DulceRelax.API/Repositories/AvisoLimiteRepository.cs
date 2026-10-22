using Google.Cloud.Firestore;

namespace DulceRelax.API.Repositories
{
    /// <summary>
    /// Marca las citas que ya recibieron el aviso de "solicitud sin confirmar".
    /// El ID del documento es el ID de la cita, así registrar dos veces la misma
    /// cita no duplica el aviso.
    /// </summary>
    public class AvisoLimiteRepository(FirestoreDb db)
    {
        private CollectionReference Col => db.Collection("avisosLimite");

        public async Task<bool> ExisteAsync(string citaId) =>
            (await Col.Document(citaId).GetSnapshotAsync()).Exists;

        public Task MarcarAsync(string citaId) =>
            Col.Document(citaId).SetAsync(new Dictionary<string, object>
            {
                { "Fecha", Timestamp.GetCurrentTimestamp() }
            });
    }
}
