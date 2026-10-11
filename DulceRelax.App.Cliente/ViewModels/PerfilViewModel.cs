using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.App.Cliente.Views;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class PerfilViewModel(IUsuarioService usuarios, IAuthService auth) : ObservableObject
    {
        private UsuarioDTO? _usuario;

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool hayDatos;
        [ObservableProperty] private string correo = "";
        [ObservableProperty] private string telefono = "";
        [ObservableProperty] private string direccion = "";

        [ObservableProperty, NotifyPropertyChangedFor(nameof(Iniciales))]
        private string nombre = "";

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        public string Iniciales
        {
            get
            {
                var partes = Nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return partes.Length switch
                {
                    0 => "?",
                    1 => partes[0][..1].ToUpperInvariant(),
                    _ => $"{partes[0][0]}{partes[1][0]}".ToUpperInvariant()
                };
            }
        }

        [RelayCommand]
        private async Task CargarAsync()
        {
            Cargando = true;
            Error = null;

            _usuario = await usuarios.ObtenerMeAsync();
            if (_usuario is null)
            {
                HayDatos = false;
                Error = "No se pudieron cargar tus datos.";
            }
            else
            {
                Nombre = _usuario.NombreCompleto ?? "";
                Correo = _usuario.Correo ?? "";
                Telefono = _usuario.NumTelefono ?? "";
                Direccion = _usuario.Direccion ?? "";
                HayDatos = true;
            }

            Cargando = false;
        }

        [RelayCommand]
        private Task EditarAsync() =>
            _usuario is null
                ? Task.CompletedTask
                : Shell.Current.GoToAsync(nameof(PerfilClienteEdit),
                    new Dictionary<string, object> { ["Usuario"] = _usuario });

        [RelayCommand]
        private async Task CambiarPasswordAsync()
        {
            if (string.IsNullOrEmpty(Correo)) return;

            var enviar = await Shell.Current.DisplayAlert("Cambiar contraseña",
                $"Te enviaremos un correo a {Correo} con un enlace para crear una nueva contraseña.",
                "Enviar", "Cancelar");
            if (!enviar) return;

            var err = await auth.RecuperarPasswordAsync(Correo);
            await Shell.Current.DisplayAlert(
                err is null ? "Correo enviado" : "Error",
                err ?? "Revisa tu bandeja de entrada.",
                "OK");
        }

        [RelayCommand]
        private async Task CerrarSesionAsync()
        {
            var cerrar = await Shell.Current.DisplayAlert("Cerrar sesión",
                "¿Quieres cerrar tu sesión?", "Sí, cerrar", "No");
            if (!cerrar) return;

            auth.CerrarSesion();

            _usuario = null;
            HayDatos = false;
            Nombre = Correo = Telefono = Direccion = "";

            await Shell.Current.GoToAsync("//InicioSesion");
        }
    }
}