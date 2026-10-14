using DulceRelax.App.Admin.Views;

namespace DulceRelax.App.Admin
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MasajeFormAdmin), typeof(MasajeFormAdmin));
        }
    }
}