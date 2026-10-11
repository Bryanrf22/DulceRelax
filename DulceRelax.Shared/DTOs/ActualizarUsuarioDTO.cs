using System.ComponentModel.DataAnnotations;

namespace DulceRelax.Shared.DTOs
{
    public class ActualizarUsuarioDTO
    {
        [Required, StringLength(80, MinimumLength = 3)]
        public string NombreCompleto { get; set; }

        [Required, Phone, StringLength(20)]
        public string NumTelefono { get; set; }

        [Required, StringLength(200, MinimumLength = 5)]
        public string Direccion { get; set; }
    }
}
