using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.Shared.DTOs;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class SolicitudesViewModel(ICitaService citaService) : ObservableObject
    {
        public ObservableCollection<CitaItem> Citas { get; } = [];

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinSolicitudes;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        [RelayCommand]
        private async Task CargarAsync()
        {
            Cargando = true;
            Error = null;

            var (lista, err) = await citaService.ObtenerPendientesAsync();

            Citas.Clear();
            foreach (var c in lista) Citas.Add(new CitaItem(c));

            Error = err;
            SinSolicitudes = err is null && Citas.Count == 0;
            Cargando = false;
        }

        [RelayCommand]
        private Task ConfirmarAsync(CitaItem item) =>
            CambiarEstadoAsync(item, EstadoCita.Confirmada,
                "Confirmar cita", $"¿Confirmar la cita de {item.Cliente}?", "Sí, confirmar");

        [RelayCommand]
        private Task RechazarAsync(CitaItem item) =>
            CambiarEstadoAsync(item, EstadoCita.Cancelada,
                "Rechazar solicitud", $"¿Rechazar la solicitud de {item.Cliente}?", "Sí, rechazar");

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