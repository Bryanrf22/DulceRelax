using CommunityToolkit.Mvvm.ComponentModel;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class DiaHorarioItem : ObservableObject
    {
        public string Clave { get; }
        public string Nombre { get; }

        [ObservableProperty] private bool activo;
        [ObservableProperty] private TimeSpan inicio;
        [ObservableProperty] private TimeSpan fin;

        public DiaHorarioItem(string clave, string nombre, bool esActivo, TimeSpan desde, TimeSpan hasta)
        {
            Clave = clave;
            Nombre = nombre;
            Activo = esActivo;
            Inicio = desde;
            Fin = hasta;
        }
    }
}