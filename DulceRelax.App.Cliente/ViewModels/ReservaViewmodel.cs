using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class ReservaViewModel(ICitaService citas, IUsuarioService usuarios, IMasajeService masajes)
        : ObservableObject, IQueryAttributable
    {
        private UsuarioDTO? _usuario;
        private RespuestaCitaDTO? _cita;

        public bool EsEdicion => _cita is not null;
        public string Titulo => EsEdicion ? "Editar cita" : "Solicitar cita";
        public string TextoBoton => EsEdicion ? "Guardar cambios" : "Solicitar cita";
        public bool MostrarTelefono => !EsEdicion;
        public bool TieneDuracion => Masaje?.DuracionMinutos > 0;
        public bool HayError => Error is not null;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(TieneDuracion))]
        private MasajeDTO? masaje;

        [ObservableProperty] private DateTime fechaMinima = DateTime.Today.AddDays(5);
        [ObservableProperty] private DateTime fecha = DateTime.Today.AddDays(5);
        [ObservableProperty] private TimeSpan hora = new(10, 0, 0);
        [ObservableProperty] private string telefono = "";
        [ObservableProperty] private string direccion = "";
        [ObservableProperty] private string detalle = "";
        [ObservableProperty] private bool enviando;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Cita", out var c) && c is RespuestaCitaDTO cita)
            {
                _ = CargarEdicionAsync(cita);
                return;
            }

            if (query.TryGetValue("Masaje", out var valor) && valor is MasajeDTO seleccionado)
                Masaje = seleccionado;

            _ = CargarPerfilAsync();
        }

        private async Task CargarEdicionAsync(RespuestaCitaDTO cita)
        {
            _cita = cita;
            OnPropertyChanged(nameof(EsEdicion));
            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(TextoBoton));
            OnPropertyChanged(nameof(MostrarTelefono));

            var local = cita.FechaHora.ToLocalTime();
            var minimaNormal = DateTime.Today.AddDays(5);

            FechaMinima = local.Date < minimaNormal ? local.Date : minimaNormal;
            Fecha = local.Date;
            Hora = local.TimeOfDay;
            Direccion = cita.DireccionExacta;
            Detalle = cita.DetalleDireccion ?? "";

            Masaje = await masajes.ObtenerAsync(cita.MasajeId)
                ?? new MasajeDTO { Id = cita.MasajeId, Nombre = cita.MasajeNombre };
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

            if (string.IsNullOrWhiteSpace(Direccion))
            {
                Error = "Completa la dirección exacta.";
                return;
            }

            var local = DateTime.SpecifyKind(Fecha.Date + Hora, DateTimeKind.Local);
            var utc = local.ToUniversalTime();

            var cambioFecha = !EsEdicion ||
                Math.Abs((utc - _cita!.FechaHora.ToUniversalTime()).TotalMinutes) >= 1;

            if (cambioFecha && local < DateTime.Now.AddDays(5))
            {
                Error = "La cita debe solicitarse con al menos 5 días de anticipación.";
                return;
            }

            Enviando = true;
            Error = EsEdicion ? await ActualizarCitaAsync(utc) : await CrearCitaAsync(utc);
            Enviando = false;

            if (Error is null)
            {
                await Shell.Current.DisplayAlert(
                    EsEdicion ? "Cita actualizada" : "Cita solicitada",
                    EsEdicion ? "Tus cambios fueron guardados." : "Tu solicitud quedó pendiente de confirmación.",
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        private async Task<string?> CrearCitaAsync(DateTime utc)
        {
            if (_usuario is null && !await CargarPerfilAsync())
                return "No se pudieron cargar tus datos. Intenta de nuevo.";

            if (string.IsNullOrWhiteSpace(Telefono))
                return "Completa tu teléfono.";

            return await citas.CrearAsync(new RegistrarCitaDTO
            {
                UsuarioId = _usuario!.Id,
                UsuarioNombre = _usuario.NombreCompleto,
                UsuarioTelefono = Telefono.Trim(),
                DireccionExacta = Direccion.Trim(),
                DetalleDireccion = string.IsNullOrWhiteSpace(Detalle) ? null : Detalle.Trim(),
                MasajeId = Masaje!.Id,
                MasajeNombre = Masaje.Nombre,
                FechaHora = utc
            });
        }

        private Task<string?> ActualizarCitaAsync(DateTime utc) =>
            citas.ActualizarAsync(_cita!.Id, new ActualizarCitaDTO
            {
                Id = _cita.Id,
                DireccionExacta = Direccion.Trim(),
                DetalleDireccion = string.IsNullOrWhiteSpace(Detalle) ? null : Detalle.Trim(),
                MasajeId = _cita.MasajeId,
                MasajeNombre = _cita.MasajeNombre,
                FechaHora = utc,
                Estado = EstadoCita.Pendiente
            });
    }
}