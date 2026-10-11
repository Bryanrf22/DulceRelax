using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class PerfilEditViewModel(IUsuarioService usuarios)
        : ObservableObject, IQueryAttributable
    {
        [ObservableProperty] private string nombre = "";
        [ObservableProperty] private string telefono = "";
        [ObservableProperty] private string direccion = "";
        [ObservableProperty] private string correo = "";
        [ObservableProperty] private bool guardando;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Usuario", out var valor) && valor is UsuarioDTO u)
            {
                Nombre = u.NombreCompleto ?? "";
                Telefono = u.NumTelefono ?? "";
                Direccion = u.Direccion ?? "";
                Correo = u.Correo ?? "";
            }
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            var n = Nombre.Trim();
            var t = Telefono.Trim();
            var d = Direccion.Trim();

            Error = Validar(n, t, d);
            if (Error is not null) return;

            Guardando = true;
            Error = await usuarios.ActualizarMeAsync(new ActualizarUsuarioDTO
            {
                NombreCompleto = n,
                NumTelefono = t,
                Direccion = d
            });
            Guardando = false;

            if (Error is null)
            {
                await Shell.Current.DisplayAlert("Perfil actualizado", "Tus datos fueron guardados.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private Task CancelarAsync() => Shell.Current.GoToAsync("..");

        private static string? Validar(string nombre, string telefono, string direccion)
        {
            if (!Regex.IsMatch(nombre, @"^[\p{L}\p{M}][\p{L}\p{M}' .-]{2,79}$"))
                return "Ingresa un nombre válido (3 a 80 letras).";

            var digitos = telefono.Count(char.IsDigit);
            if (!Regex.IsMatch(telefono, @"^\+?[\d\s-]{8,20}$") || digitos is < 8 or > 15)
                return "Ingresa un teléfono válido (8 a 15 dígitos).";

            if (direccion.Length is < 5 or > 200)
                return "La dirección debe tener entre 5 y 200 caracteres.";

            return null;
        }
    }
}