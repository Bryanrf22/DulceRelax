using DulceRelax.App.Cliente.Views;

namespace DulceRelax.App.Cliente
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rutas de las vistas que se apilan sobre la raíz (inicio de sesión)
            Routing.RegisterRoute(nameof(RegistroCliente), typeof(RegistroCliente));
            Routing.RegisterRoute(nameof(RecuperarContraseñaCliente), typeof(RecuperarContraseñaCliente));
            Routing.RegisterRoute(nameof(PrincipalCliente), typeof(PrincipalCliente));
            Routing.RegisterRoute(nameof(ReservaMasajeCliente), typeof(ReservaMasajeCliente));
            Routing.RegisterRoute(nameof(MisCitasCliente), typeof(MisCitasCliente));
            Routing.RegisterRoute(nameof(PerfilCliente), typeof(PerfilCliente));
            Routing.RegisterRoute(nameof(PerfilClienteEdit), typeof(PerfilClienteEdit));
        }
    }
}