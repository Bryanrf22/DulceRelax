using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace DulceRelax.Shared.DTOs
{
    [FirestoreData]
    public class MasajeDTO
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Nombre { get; set; }

        [FirestoreProperty]
        public string Descripcion { get; set; }

        [FirestoreProperty]
        public bool Disponible { get; set; }

        [FirestoreProperty]
        public double Precio {  get; set; }

    }
}
