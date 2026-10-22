using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class AjustesViewModel(IHorarioService horarioService, IAuthService auth, PushService push)
        : ObservableObject
    {
        private static readonly (string Clave, string Nombre)[] Orden =
        [
            ("Monday", "Lunes"), ("Tuesday", "Martes"), ("Wednesday", "Miércoles"),
            ("Thursday", "Jueves"), ("Friday", "Viernes"), ("Saturday", "Sábado"),
            ("Sunday", "Domingo")
        ];

        public ObservableCollection<DiaHorarioItem> Dias { get; } = [];
        public bool Cargado { get; private set; }

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinConfigurar;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        [RelayCommand]
        private async Task CargarAsync()
        {
            Cargando = true;
            Error = null;

            var (h, err) = await horarioService.ObtenerAsync();
            if (err is not null)
            {
                Error = err;
                Cargando = false;
                return;
            }

            SinConfigurar = h?.Dias is null;

            Dias.Clear();
            foreach (var (clave, nombre) in Orden)
            {
                if (h?.Dias is not null && h.Dias.TryGetValue(clave, out var d))
                    Dias.Add(new DiaHorarioItem(clave, nombre, d.Activo,
                        Leer(d.HoraInicio, 8), Leer(d.HoraFin, 17)));
                else
                    Dias.Add(new DiaHorarioItem(clave, nombre, clave != "Sunday",
                        new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)));
            }

            Cargado = true;
            Cargando = false;
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            Error = null;

            var invalido = Dias.FirstOrDefault(d => d.Activo && d.Inicio >= d.Fin);
            if (invalido is not null)
            {
                Error = $"La hora de apertura del {invalido.Nombre.ToLower()} debe ser anterior a la de cierre.";
                return;
            }

            var dto = new HorarioAtencionDTO
            {
                Dias = Dias.ToDictionary(d => d.Clave, d => new DiaHorarioDTO
                {
                    Activo = d.Activo,
                    HoraInicio = d.Inicio.ToString(@"hh\:mm"),
                    HoraFin = d.Fin.ToString(@"hh\:mm")
                })
            };

            Error = await horarioService.GuardarAsync(dto);
            if (Error is not null) return;

            SinConfigurar = false;
            await Shell.Current.DisplayAlert("Listo", "Horario guardado.", "OK");
        }

        [RelayCommand]
        private async Task CerrarSesionAsync()
        {
            // Se da de baja el dispositivo antes de borrar el token de autenticación,
            // porque la baja también necesita ir autenticada.
            await push.DetenerAsync();

            auth.CerrarSesion();
            await Shell.Current.GoToAsync("//InicioSesion");
        }

        private static TimeSpan Leer(string? texto, int horaPorDefecto) =>
            TimeSpan.TryParse(texto, out var t) ? t : new TimeSpan(horaPorDefecto, 0, 0);
    }
}