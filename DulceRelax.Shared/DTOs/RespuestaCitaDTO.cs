namespace DulceRelax.Shared.DTOs
{
    public class CitaRespuestaDTO
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioTelefono { get; set; }
        public string DireccionExacta { get; set; }
        public string? DetalleDireccion { get; set; }
        public string MasajeId { get; set; }
        public string MasajeNombre { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoCita Estado { get; set; }
    }
}