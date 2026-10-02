using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DulceRelax.Shared.DTOs
{
    public class ActualizarCitaDTO
    {
        [Required] public string Id { get; set; }
        [Required] public string DireccionExacta { get; set; }
        public string? DetalleDireccion { get; set; }
        [Required] public string MasajeId { get; set; }
        [Required] public string MasajeNombre { get; set; }
        [Required] public DateTime FechaHora { get; set; }
        [Required] public EstadoCita Estado { get; set; }
    }
}
