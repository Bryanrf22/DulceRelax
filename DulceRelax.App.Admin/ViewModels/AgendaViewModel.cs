using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class AgendaViewModel(ICitaService citaService, ICitaCacheService cache, ISincronizadorService sync) : ObservableObject
    {
        public ObservableCollection<CitaItem> Citas { get; } = [];

        [ObservableProperty, NotifyPropertyChangedFor(nameof(DiaTexto))]
        private DateTime dia = DateTime.Today;

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinCitas;
        [ObservableProperty] private bool sinConexion;
        [ObservableProperty] private string avisoSinConexion = "";

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        public string DiaTexto =>
            (Dia.Date == DateTime.Today ? "Hoy · " : "") + Dia.ToString("ddd d MMM");

        [RelayCommand]
        private async Task CargarAsync()
        {
            Cargando = true;
            Error = null;
            SinConexion = false;

            var (lista, err) = await citaService.ObtenerPorDiaAsync(Dia);

            if (err is not null && Red.EsError(err) && cache.CubreDia(Dia))
            {
                lista = await cache.ObtenerDiaAsync(Dia);
                SinConexion = true;
                err = null;

                var sync = cache.UltimaSincronizacion?.ToLocalTime();
                AvisoSinConexion = "Sin conexión · se muestran solo las citas confirmadas " +
                                   $"guardadas (actualizado {sync:ddd d MMM, h:mm tt}).";
            }

            Citas.Clear();
            foreach (var c in lista) Citas.Add(new CitaItem(c, soloLectura: SinConexion));

            Error = err;
            SinCitas = err is null && Citas.Count == 0;
            Cargando = false;

            if (err is null && !SinConexion) _ = sync.SincronizarAsync();
        }

        [RelayCommand]
        private async Task DiaAnteriorAsync() { Dia = Dia.AddDays(-1); await CargarAsync(); }

        [RelayCommand]
        private async Task DiaSiguienteAsync() { Dia = Dia.AddDays(1); await CargarAsync(); }

        [RelayCommand]
        private async Task HoyAsync() { Dia = DateTime.Today; await CargarAsync(); }

        [RelayCommand]
        private Task ConfirmarAsync(CitaItem item) =>
            CambiarEstadoAsync(item, EstadoCita.Confirmada,
                "Confirmar cita", $"¿Confirmar la cita de {item.Cliente}?", "Sí, confirmar");

        [RelayCommand]
        private Task CancelarAsync(CitaItem item) =>
            CambiarEstadoAsync(item, EstadoCita.Cancelada,
                "Cancelar cita", $"¿Cancelar la cita de {item.Cliente}?", "Sí, cancelar");

        private async Task CambiarEstadoAsync(CitaItem item, EstadoCita estado,
            string titulo, string mensaje, string aceptar)
        {
            if (!await Shell.Current.DisplayAlert(titulo, mensaje, aceptar, "No")) return;

            var err = await citaService.CambiarEstadoAsync(item.Cita.Id, estado);
            if (err is not null)
            {
                await Shell.Current.DisplayAlert("Error", err, "OK");
                return;
            }

            await CargarAsync();
        }
    }
}