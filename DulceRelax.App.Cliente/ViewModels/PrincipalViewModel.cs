using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.App.Cliente.Views;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class PrincipalViewModel(IMasajeService masajeService) : ObservableObject
    {
        public ObservableCollection<MasajeDTO> Masajes { get; } = [];

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinMasajes;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        [RelayCommand]
        private async Task CargarAsync()
        {
            if (Cargando) return;
            Cargando = true;
            Error = null;

            var (lista, err) = await masajeService.ObtenerDisponiblesAsync();

            Masajes.Clear();
            foreach (var m in lista) Masajes.Add(m);
            Error = err;
            SinMasajes = err is null && Masajes.Count == 0;
            Cargando = false;
        }

        [RelayCommand]
        private Task SolicitarCitaAsync(MasajeDTO masaje) =>
            Shell.Current.GoToAsync(nameof(ReservaMasajeCliente),
                new Dictionary<string, object> { ["Masaje"] = masaje });
    }
}