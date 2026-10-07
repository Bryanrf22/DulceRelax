namespace DulceRelax.App.Cliente.Views;

public partial class PrincipalCliente : ContentPage
{
    public PrincipalCliente()
    {
        InitializeComponent();
    }

    private void OnRelaxEntered(object sender, PointerEventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#2F7047");
        }
    }

    private void OnRelaxExited(object sender, PointerEventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#3F8758");
        }
    }

    // TODO: pantalla de reserva de masaje (pendiente de crear)
    private void OnRelaxClicked(object sender, EventArgs e)
    {
    }

    private void OnDeepEntered(object sender, PointerEventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#2F7047");
        }
    }

    private void OnDeepExited(object sender, PointerEventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#3F8758");
        }
    }

    // TODO: pantalla de reserva de masaje (pendiente de crear)
    private void OnDeepClicked(object sender, EventArgs e)
    {
    }

    private void OnHomeEntered(object sender, PointerEventArgs e)
    {
        HomeLabel.TextColor = Color.FromArgb("#2F7047");
    }

    private void OnHomeExited(object sender, PointerEventArgs e)
    {
        HomeLabel.TextColor = Color.FromArgb("#3F8758");
    }

    private void OnCalendarEntered(object sender, PointerEventArgs e)
    {
        CalendarLabel.TextColor = Color.FromArgb("#3F8758");
    }

    private void OnCalendarExited(object sender, PointerEventArgs e)
    {
        CalendarLabel.TextColor = Color.FromArgb("#333333");
    }

    private void OnProfileEntered(object sender, PointerEventArgs e)
    {
        ProfileLabel.TextColor = Color.FromArgb("#3F8758");
    }

    private void OnProfileExited(object sender, PointerEventArgs e)
    {
        ProfileLabel.TextColor = Color.FromArgb("#333333");
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PerfilCliente));
    }
}
