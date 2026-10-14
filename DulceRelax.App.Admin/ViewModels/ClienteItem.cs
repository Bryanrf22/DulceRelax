using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public class ClienteItem(UsuarioDTO usuario)
    {
        public string Nombre => usuario.NombreCompleto;
        public string Telefono => usuario.NumTelefono;
        public string TelefonoTexto => $"☎ {usuario.NumTelefono}";
        public string Correo => usuario.Correo;
        public string Direccion => usuario.Direccion;
        public bool TieneTelefono => !string.IsNullOrWhiteSpace(usuario.NumTelefono);
        public bool TieneDireccion => !string.IsNullOrWhiteSpace(usuario.Direccion);

        public bool Coincide(string texto) =>
            Contiene(usuario.NombreCompleto, texto)
            || Contiene(usuario.NumTelefono, texto)
            || Contiene(usuario.Correo, texto);

        private static bool Contiene(string? origen, string texto) =>
            origen?.Contains(texto, StringComparison.OrdinalIgnoreCase) == true;
    }
}