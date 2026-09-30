using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DulceRelax.Shared.DTOs
{
    public class ActualizarMasajeDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Disponible { get; set; }
        public double Precio { get; set; }
    }
}
