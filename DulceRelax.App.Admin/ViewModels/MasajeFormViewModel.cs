using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.Shared.DTOs;
using System.Globalization;
using static Android.Icu.Text.CaseMap;
using static Android.Util.EventLogTags;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class MasajeFormViewModel(IMasajeService masajeService)
        : ObservableObject, IQueryAttributable
    {
        private string? _id;

        [ObservableProperty] private string titulo = "Nuevo masaje";
        [ObservableProperty] private bool esEdicion;
        [ObservableProperty] private string nombre = "";
        [ObservableProperty] private string descripcion = "";
        [ObservableProperty] private string precioTexto = "";
        [ObservableProperty] private string duracionTexto = "";
        [ObservableProperty] private string imagenUrl = "";
        [ObservableProperty] private bool disponible = true;
        [ObservableProperty] private string? error;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("Masaje", out var o) || o is not MasajeDTO m) return;

            _id = m.Id;
            EsEdicion = true;
            Titulo = "Editar masaje";
            Nombre = m.Nombre;
            Descripcion = m.Descripcion ?? "";
            PrecioTexto = m.Precio.ToString(CultureInfo.InvariantCulture);
            DuracionTexto = m.DuracionMinutos.ToString();
            ImagenUrl = m.ImagenUrl ?? "";
            Disponible = m.Disponible;
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            Error = null;

            if (string.IsNullOrWhiteSpace(Nombre)) { Error = "Ingresa el nombre del masaje."; return; }

            if (!double.TryParse(PrecioTexto.Trim().Replace(',', '.'), NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var precio) || precio <= 0)
            { Error = "Ingresa un precio válido."; return; }

            if (!int.TryParse(DuracionTexto.Trim(), out var duracion) || duracion <= 0)
            { Error = "Ingresa una duración válida en minutos."; return; }

            var url = string.IsNullOrWhiteSpace(ImagenUrl) ? null : ImagenUrl.Trim();

            Error = EsEdicion
                ? await masajeService.ActualizarAsync(_id!, new ActualizarMasajeDTO
                {
                    Nombre = Nombre.Trim(),
                    Descripcion = Descripcion.Trim(),
                    Precio = precio,
                    DuracionMinutos = duracion,
                    ImagenUrl = url,
                    Disponible = Disponible
                })
                : await masajeService.CrearAsync(new RegistrarMasajeDTO
                {
                    Nombre = Nombre.Trim(),
                    Descripcion = Descripcion.Trim(),
                    Precio = precio,
                    DuracionMinutos = duracion,
                    ImagenUrl = url
                });

            if (Error is null) await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task VolverAsync() => Shell.Current.GoToAsync("..");
    }
}