using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class ClientesViewModel(IUsuarioService usuarioService, IClienteCacheService cache) : ObservableObject
    {
        private List<ClienteItem> _todos = [];

        public ObservableCollection<ClienteItem> Clientes { get; } = [];

        [ObservableProperty] private bool cargando;
        [ObservableProperty] private bool sinResultados;
        [ObservableProperty] private string busqueda = "";
        [ObservableProperty] private string resumen = "";
        [ObservableProperty] private bool sinConexion;
        [ObservableProperty] private string avisoSinConexion = "";

        [ObservableProperty, NotifyPropertyChangedFor(nameof(HayError))]
        private string? error;

        public bool HayError => Error is not null;

        partial void OnBusquedaChanged(string value) => Filtrar();

        [RelayCommand]
        private async Task CargarAsync()
        {
            Cargando = true;
            Error = null;
            SinConexion = false;

            var (lista, err) = await usuarioService.ObtenerTodosAsync();

            if (err is not null && Red.EsError(err) && cache.UltimaSincronizacion is not null)
            {
                lista = await cache.ObtenerTodosAsync();
                SinConexion = true;
                err = null;

                var sync = cache.UltimaSincronizacion?.ToLocalTime();
                AvisoSinConexion = $"Sin conexión · lista guardada (actualizada {sync:ddd d MMM, h:mm tt}).";
            }
            else if (err is null)
            {
                try { await cache.GuardarAsync(lista); } catch { }
            }

            _todos = lista.Select(u => new ClienteItem(u))
                          .OrderBy(c => c.Nombre, StringComparer.CurrentCultureIgnoreCase)
                          .ToList();
            Error = err;
            Filtrar();
            Cargando = false;
        }

        private void Filtrar()
        {
            var q = Busqueda.Trim();
            var lista = q.Length == 0 ? _todos : _todos.Where(c => c.Coincide(q)).ToList();

            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);

            SinResultados = Error is null && Clientes.Count == 0;
            Resumen = Error is null ? $"{Clientes.Count} cliente(s)" : "";
        }

        [RelayCommand]
        private async Task LlamarAsync(ClienteItem item)
        {
            try
            {
                if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open(item.Telefono);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir el marcador.", "OK");
            }
        }
    }
}