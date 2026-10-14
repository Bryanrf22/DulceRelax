using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class RegistroCliente : ContentPage
{
    public RegistroCliente(RegistroViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnTogglePassword(object sender, TappedEventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        var clave = PasswordEntry.IsPassword ? "IconoVer" : "IconoOcultar";
        if (Application.Current?.Resources.TryGetValue(clave, out var glyph) == true && glyph is string texto)
            PasswordToggle.Text = texto;
    }
}