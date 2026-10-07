using DulceRelax.App.Cliente.Views;

namespace DulceRelax.App.Cliente
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rutas de las pantallas de detalle (se apilan sobre la raíz)
            Routing.RegisterRoute(nameof(RegistroCliente), typeof(RegistroCliente));
            Routing.RegisterRoute(nameof(PrincipalCliente), typeof(PrincipalCliente));
            Routing.RegisterRoute(nameof(PerfilCliente), typeof(PerfilCliente));
            Routing.RegisterRoute(nameof(PerfilClienteEdit), typeof(PerfilClienteEdit));
        }
    }
}
