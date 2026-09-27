using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace DulceRelax.Shared.DTOs
{
    [FirestoreData]
    public class UsuarioDTO
    {
        [FirestoreDocumentId]
        public string Id { get; set; } 

        [FirestoreProperty]
        public string NombreCompleto { get; set; }

        [FirestoreProperty]
        public string NumTelefono { get; set; }

        [FirestoreProperty]
        public string Correo {  get; set; }
    }
}
