using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.App.Cliente.Views;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public class CitaItem(RespuestaCitaDTO cita)
    {
        private readonly DateTime _local = cita.FechaHora.ToLocalTime();
        private bool EsFutura => cita.FechaHora.ToUniversalTime() > DateTime.UtcNow;

        public RespuestaCitaDTO Cita => cita;
        public string MasajeNombre => cita.MasajeNombre;
        public string FechaHoraTexto => $"◷ {_local:ddd d MMM yyyy} · {_local:h:mm tt}";
        public string Direccion => $"{cita.DireccionExacta}";
        public string? Detalle => cita.DetalleDireccion;
        public bool TieneDetalle => !string.IsNullOrWhiteSpace(cita.DetalleDireccion);
        public string EstadoTexto => cita.Estado.ToString();

        public bool PuedeEditar => cita.Estado == EstadoCita.Pendiente && EsFutura;
        public bool PuedeCancelar => cita.Estado != EstadoCita.Cancelada && EsFutura;
        public bool TieneAcciones => PuedeEditar || PuedeCancelar;

        public Color EstadoFondo => cita.Estado switch
        {
            EstadoCita.Confirmada => Color.FromArgb("#EAF4EC"),
            EstadoCita.Cancelada => Color.FromArgb("#FDECEA"),
            _ => Color.FromArgb("#FFF3CD")
        };

        public Color EstadoColor => cita.Estado switch
        {
            EstadoCita.Confirmada => Color.FromArgb("#3F8758"),
            EstadoCita.Cancelada => Color.FromArgb("#B3261E"),
            _ => Color.FromArgb("#8A6D3B")
        };
    }

    public partial class MisCitasViewModel(ICitaService citaService) : ObservableObject
    {
        public ObservableCollection<CitaItem> Citas { get; } = [];

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinCitas;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        [RelayCommand]
        private async Task CargarAsync()
        {
            Cargando = true;
            Error = null;

            var (lista, err) = await citaService.ObtenerMiasAsync();

            var ahora = DateTime.UtcNow;
            var proximas = lista
                .Where(c => c.Estado != EstadoCita.Cancelada && c.FechaHora.ToUniversalTime() >= ahora)
                .OrderBy(c => c.FechaHora)
                .ToList();
            var resto = lista.Except(proximas).OrderByDescending(c => c.FechaHora);

            Citas.Clear();
            foreach (var c in proximas.Concat(resto)) Citas.Add(new CitaItem(c));

            Error = err;
            SinCitas = err is null && Citas.Count == 0;
            Cargando = false;
        }

        [RelayCommand]
        private Task EditarAsync(CitaItem item) =>
            Shell.Current.GoToAsync(nameof(ReservaMasajeCliente),
                new Dictionary<string, object> { ["Cita"] = item.Cita });

        [RelayCommand]
        private async Task CancelarAsync(CitaItem item)
        {
            var confirmar = await Shell.Current.DisplayAlert(
                "Cancelar cita", $"¿Cancelar tu cita de {item.MasajeNombre}?", "Sí, cancelar", "No");
            if (!confirmar) return;

            var err = await citaService.CancelarAsync(item.Cita.Id);
            if (err is not null)
            {
                await Shell.Current.DisplayAlert("Error", err, "OK");
                return;
            }

            await CargarAsync();
        }
    }
}