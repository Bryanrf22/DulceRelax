using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.App.Admin.Views;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class MasajesViewModel(IMasajeService masajeService) : ObservableObject
    {
        public ObservableCollection<MasajeItem> Masajes { get; } = [];

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinMasajes;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        [RelayCommand]
        private async Task CargarAsync()
        {
            Cargando = true;
            Error = null;

            var (lista, err) = await masajeService.ObtenerTodosAsync();

            Masajes.Clear();
            foreach (var m in lista.OrderByDescending(m => m.Disponible).ThenBy(m => m.Nombre))
                Masajes.Add(new MasajeItem(m));

            Error = err;
            SinMasajes = err is null && Masajes.Count == 0;
            Cargando = false;
        }

        [RelayCommand]
        private Task NuevoAsync() => Shell.Current.GoToAsync(nameof(MasajeFormAdmin));

        [RelayCommand]
        private Task EditarAsync(MasajeItem item) =>
            Shell.Current.GoToAsync(nameof(MasajeFormAdmin),
                new Dictionary<string, object> { ["Masaje"] = item.Masaje });

        [RelayCommand]
        private async Task AlternarAsync(MasajeItem item)
        {
            var m = item.Masaje;
            var err = await masajeService.ActualizarAsync(m.Id, new ActualizarMasajeDTO
            {
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Precio = m.Precio,
                DuracionMinutos = m.DuracionMinutos,
                ImagenUrl = m.ImagenUrl,
                Disponible = !m.Disponible
            });

            if (err is not null)
            {
                await Shell.Current.DisplayAlert("Error", err, "OK");
                return;
            }

            await CargarAsync();
        }
    }
}