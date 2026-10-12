using DulceRelax.App.Admin.Views;

namespace DulceRelax.App.Admin;

public partial class AppShell : Shell
{
    public AppShell(InicioSesionAdmin inicioSesion)
    {
        InitializeComponent();

        Items.Add(new ShellContent
        {
            Route = "InicioSesion",
            Content = inicioSesion
        });

        Routing.RegisterRoute(
            nameof(PrincipalAdmin),
            typeof(PrincipalAdmin));
    }
}