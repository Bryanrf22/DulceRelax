using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace DulceRelax.Shared.DTOs
{
    [FirestoreData]
    public class DiaHorarioDTO
    {
        [FirestoreProperty]
        public bool Activo { get; set; }

        [FirestoreProperty]
        public string HoraInicio { get; set; }

        [FirestoreProperty]
        public string HoraFin { get; set; }
    }
}
