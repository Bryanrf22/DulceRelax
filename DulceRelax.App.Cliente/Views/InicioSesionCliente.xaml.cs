using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views
{
    public partial class InicioSesionCliente : ContentPage
    {
        public InicioSesionCliente(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync(nameof(RecuperarContraseñaCliente));

        private async void OnRegisterClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync(nameof(RegistroCliente));

        private void OnTogglePassword(object sender, EventArgs e)
        {
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
            ToggleButton.Text = PasswordEntry.IsPassword ? "Mostrar" : "Ocultar";
            SemanticProperties.SetDescription(ToggleButton, PasswordEntry.IsPassword
                ? "Mostrar contraseña" : "Ocultar contraseña");
        }
    }
}