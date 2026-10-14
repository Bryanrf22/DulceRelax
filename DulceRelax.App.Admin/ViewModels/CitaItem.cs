using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public class CitaItem(RespuestaCitaDTO cita, bool soloLectura = false)
    {
        private readonly DateTime _local = cita.FechaHora.ToLocalTime();
        private bool EsFutura => cita.FechaHora.ToUniversalTime() > DateTime.UtcNow;

        public RespuestaCitaDTO Cita => cita;
        public string MasajeNombre => cita.MasajeNombre;
        public string Cliente => cita.UsuarioNombre;
        public string Telefono => cita.UsuarioTelefono;
        public bool TieneTelefono => !string.IsNullOrWhiteSpace(cita.UsuarioTelefono);
        public string HoraTexto => $"{_local:h:mm tt}";
        public string FechaHoraTexto => $"{_local:ddd d MMM} · {_local:h:mm tt}";
        public string Direccion => cita.DireccionExacta;
        public string? Detalle => cita.DetalleDireccion;
        public bool TieneDetalle => !string.IsNullOrWhiteSpace(cita.DetalleDireccion);
        public string EstadoTexto => cita.Estado.ToString();

        public bool PuedeConfirmar => !soloLectura && cita.Estado == EstadoCita.Pendiente && EsFutura;
        public bool PuedeCancelar => !soloLectura && cita.Estado != EstadoCita.Cancelada && EsFutura;
        public bool TieneAcciones => PuedeConfirmar || PuedeCancelar;

        public Color EstadoFondo => cita.Estado switch
        {
            EstadoCita.Confirmada => Color.FromArgb("#EAF4EC"),
            EstadoCita.Cancelada => Color.FromArgb("#FDECEA"),
            _ => Color.FromArgb("#FFF3CD")
        };

        public Color EstadoColor => cita.Estado switch
        {
            EstadoCita.Confirmada => Color.FromArgb("#3F8758"),
            EstadoCita.Cancelada => Color.FromArgb("#B3261E"),
            _ => Color.FromArgb("#8A6D3B")
        };
    }
}