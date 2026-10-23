using Microsoft.Maui.Controls;
using DulceRelax.UI.Behaviors;

namespace DulceRelax.App.Cliente.Views
{
    public partial class InicioSesionCliente : ContentPage
    {
        public InicioSesionCliente()
        {
            InitializeComponent();
            
            // Apply SafeAreaBehavior to this page
            var safeAreaBehavior = new SafeAreaBehavior
            {
                Edges = SafeAreaEdges.All
            };
            
            // Attach the behavior to the page
            Behaviors.Add(safeAreaBehavior);
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync(nameof(RecuperarContraseñaCliente));

        private async void OnRegisterClicked(object sender, EventArgs e) =>
            await Shell.Current.GoToAsync(nameof(RegistroCliente));

        private void OnTogglePassword(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.SemanticProperties.Description = btn.Text == "Mostrar"
                    ? "Ocultar contraseña" : "Mostrar contraseña";
            }
        }
    }
}