using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.Shared.DTOs;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class AgendaViewModel(ICitaService citaService) : ObservableObject
    {
        public ObservableCollection<CitaItem> Citas { get; } = [];

        [ObservableProperty, NotifyPropertyChangedFor(nameof(DiaTexto))]
        private DateTime dia = DateTime.Today;

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinCitas;

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

            var (lista, err) = await citaService.ObtenerPorDiaAsync(Dia);

            Citas.Clear();
            foreach (var c in lista) Citas.Add(new CitaItem(c));

            Error = err;
            SinCitas = err is null && Citas.Count == 0;
            Cargando = false;
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