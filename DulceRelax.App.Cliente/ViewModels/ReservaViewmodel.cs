using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class ReservaViewModel(ICitaService citas, IUsuarioService usuarios)
        : ObservableObject, IQueryAttributable
    {
        private UsuarioDTO? _usuario;

        public DateTime FechaMinima { get; } = DateTime.Today.AddDays(5);

        [ObservableProperty, NotifyPropertyChangedFor(nameof(TieneDuracion))]
        private MasajeDTO? masaje;

        [ObservableProperty] private DateTime fecha = DateTime.Today.AddDays(5);
        [ObservableProperty] private TimeSpan hora = new(10, 0, 0);
        [ObservableProperty] private string telefono = "";
        [ObservableProperty] private string direccion = "";
        [ObservableProperty] private string detalle = "";
        [ObservableProperty] private bool enviando;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;
        public bool TieneDuracion => Masaje?.DuracionMinutos > 0;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Masaje", out var valor) && valor is MasajeDTO seleccionado)
                Masaje = seleccionado;

            _ = CargarPerfilAsync();
        }

        private async Task<bool> CargarPerfilAsync()
        {
            _usuario = await usuarios.ObtenerMeAsync();
            if (_usuario is null) return false;

            if (string.IsNullOrWhiteSpace(Telefono)) Telefono = _usuario.NumTelefono ?? "";
            if (string.IsNullOrWhiteSpace(Direccion)) Direccion = _usuario.Direccion ?? "";
            return true;
        }

        [RelayCommand]
        private async Task ReservarAsync()
        {
            Error = null;

            if (Masaje is null)
            {
                Error = "Selecciona un masaje desde el menú principal.";
                return;
            }

            if (_usuario is null && !await CargarPerfilAsync())
            {
                Error = "No se pudieron cargar tus datos. Intenta de nuevo.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Telefono) || string.IsNullOrWhiteSpace(Direccion))
            {
                Error = "Completa tu teléfono y la dirección exacta.";
                return;
            }

            var local = Fecha.Date + Hora;
            if (local < DateTime.Now.AddDays(5))
            {
                Error = "La cita debe solicitarse con al menos 5 días de anticipación.";
                return;
            }

            var dto = new RegistrarCitaDTO
            {
                UsuarioId = _usuario!.Id,
                UsuarioNombre = _usuario.NombreCompleto,
                UsuarioTelefono = Telefono.Trim(),
                DireccionExacta = Direccion.Trim(),
                DetalleDireccion = string.IsNullOrWhiteSpace(Detalle) ? null : Detalle.Trim(),
                MasajeId = Masaje.Id,
                MasajeNombre = Masaje.Nombre,
                FechaHora = DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime()
            };

            Enviando = true;
            Error = await citas.CrearAsync(dto);
            Enviando = false;

            if (Error is null)
            {
                await Shell.Current.DisplayAlert("Cita solicitada",
                    "Tu solicitud quedó pendiente de confirmación.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}