using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace DulceRelax.Shared.DTOs
{
    [FirestoreData]
    public class CitaDTO
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string UsuarioId { get; set; }

        [FirestoreProperty]
        public string UsuarioNombre { get; set; }

        [FirestoreProperty]
        public string UsuarioTelefono { get; set; }

        [FirestoreProperty]
        public string DireccionExacta { get; set; }

        [FirestoreProperty]
        public string DetalleDireccion {  get; set; } //ayuda para identificar la casa exacta

        [FirestoreProperty]
        public string MasajeId { get; set; }

        [FirestoreProperty]
        public string MasajeNombre { get; set; }

        [FirestoreProperty]
        public Timestamp FechaHora { get; set; }

        [FirestoreProperty]
        public EstadoCita Estado { get; set; }
    }

    public enum EstadoCita { Pendiente, Confirmada, Cancelada }
}
