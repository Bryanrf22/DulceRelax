using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class RegistroCliente : ContentPage
{
    public RegistroCliente(RegistroViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnTogglePassword(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

        if (sender is Button toggle)
        {
            var clave = PasswordEntry.IsPassword ? "IconoVer" : "IconoOcultar";
            if (Application.Current?.Resources.TryGetValue(clave, out var glyph) == true && glyph is string texto)
                toggle.Text = texto;

            toggle.SemanticProperties.Description = PasswordEntry.IsPassword
                ? "Mostrar contraseña" : "Ocultar contraseña";
        }
    }
}