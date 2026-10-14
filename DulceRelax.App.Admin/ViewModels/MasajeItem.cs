using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public class MasajeItem(MasajeDTO masaje)
    {
        public MasajeDTO Masaje => masaje;
        public string Nombre => masaje.Nombre;
        public string PrecioTexto => $"{masaje.Precio:0.##} C$";
        public string DuracionTexto => masaje.DuracionMinutos > 0 ? $"{masaje.DuracionMinutos} min" : "";
        public bool TieneDuracion => masaje.DuracionMinutos > 0;
        public string EstadoTexto => masaje.Disponible ? "Disponible" : "No disponible";
        public string AccionTexto => masaje.Disponible ? "Desactivar" : "Activar";
        public Color AccionColor => masaje.Disponible ? Color.FromArgb("#B3261E") : Color.FromArgb("#3F8758");
        public Color EstadoFondo => masaje.Disponible ? Color.FromArgb("#EAF4EC") : Color.FromArgb("#FDECEA");
        public Color EstadoColor => masaje.Disponible ? Color.FromArgb("#3F8758") : Color.FromArgb("#B3261E");
    }
}