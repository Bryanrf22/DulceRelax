namespace DulceRelax.App.Cliente.Views;

public partial class RegistroCliente : ContentPage
{
    public RegistroCliente()
    {
        InitializeComponent();
    }

    // TODO: registrar usuario en la API (fase posterior)
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        MessageLabel.TextColor = Color.FromArgb("#3F8758");
        MessageLabel.Text = "Registro completado";

        await Task.Delay(800);

        await Shell.Current.GoToAsync(nameof(PrincipalCliente));
    }

    private void OnRegisterEntered(object sender, PointerEventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#2F7047");
        }
    }

    private void OnRegisterExited(object sender, PointerEventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#3F8758");
        }
    }
}
