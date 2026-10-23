namespace DulceRelax.App.Admin;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Decisión de producto: la app es solo de tema claro.
        // Ver docs/diseno/tema.md.
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}