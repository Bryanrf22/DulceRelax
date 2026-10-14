using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class ReservaViewModel(ICitaService citas, IUsuarioService usuarios, IMasajeService masajes, IHorarioService horarios) : ObservableObject, IQueryAttributable
    {
        private UsuarioDTO? _usuario;
        private RespuestaCitaDTO? _cita;

        public bool EsEdicion => _cita is not null;
        public string Titulo => EsEdicion ? "Editar cita" : "Solicitar cita";
        public string TextoBoton => EsEdicion ? "Guardar cambios" : "Solicitar cita";
        public bool MostrarTelefono => !EsEdicion;
        public bool TieneDuracion => Masaje?.DuracionMinutos > 0;
        public bool HayError => Error is not null;

        private HorarioAtencionDTO? _horario;
        public bool HayHorarioDia => HorarioDiaTexto.Length > 0;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(TieneDuracion))]
        private MasajeDTO? masaje;

        [ObservableProperty] private DateTime fechaMinima = DateTime.Today.AddDays(5);
        [ObservableProperty, NotifyPropertyChangedFor(nameof(HorarioDiaTexto)), NotifyPropertyChangedFor(nameof(HayHorarioDia))]
        private DateTime fecha = DateTime.Today.AddDays(5);
        [ObservableProperty] private TimeSpan hora = new(10, 0, 0);
        [ObservableProperty] private string telefono = "";
        [ObservableProperty] private string direccion = "";
        [ObservableProperty] private string detalle = "";
        [ObservableProperty] private bool enviando;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public string HorarioDiaTexto
        {
            get
            {
                if (_horario?.Dias is null || _horario.Dias.Count == 0) return "";

                if (!_horario.Dias.TryGetValue(Fecha.DayOfWeek.ToString(), out var d) || !d.Activo)
                    return "No hay atención ese día.";

                return $"Horario de atención ese día: {Formato(d.HoraInicio)} – {Formato(d.HoraFin)}";
            }
        }

        private static string Formato(string hhmm) =>
            TimeSpan.TryParse(hhmm, out var t) ? DateTime.Today.Add(t).ToString("h:mm tt") : hhmm;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _ = CargarHorarioAsync();

            if (query.TryGetValue("Cita", out var c) && c is RespuestaCitaDTO cita)
            {
                _ = CargarEdicionAsync(cita);
                return;
            }

            if (query.TryGetValue("Masaje", out var valor) && valor is MasajeDTO seleccionado)
                Masaje = seleccionado;

            _ = CargarPerfilAsync();
        }

        private async Task CargarHorarioAsync()
        {
            _horario = await horarios.ObtenerAsync();
            OnPropertyChanged(nameof(HorarioDiaTexto));
            OnPropertyChanged(nameof(HayHorarioDia));
        }

        private bool DentroDeHorario(DateTime inicio, int minutos)
        {
            if (_horario?.Dias is null || _horario.Dias.Count == 0) return true;

            if (!_horario.Dias.TryGetValue(inicio.DayOfWeek.ToString(), out var d) || !d.Activo)
                return false;

            if (!TimeSpan.TryParse(d.HoraInicio, out var abre) ||
                !TimeSpan.TryParse(d.HoraFin, out var cierra))
                return false;

            var fin = inicio.AddMinutes(minutos);
            return fin.Date == inicio.Date && inicio.TimeOfDay >= abre && fin.TimeOfDay <= cierra;
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

            var minutos = Masaje.DuracionMinutos > 0 ? Masaje.DuracionMinutos : 60;
            if (cambioFecha && !DentroDeHorario(local, minutos))
            {
                Error = $"La cita ({minutos} min) debe quedar dentro del horario de atención. {HorarioDiaTexto}".Trim();
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