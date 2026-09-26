using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace DulceRelax.Shared.DTOs
{
    [FirestoreData]
    public class HorarioAtencionDTO
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public Dictionary<string, DiaHorarioDTO> Dias { get; set; }
    }
}
