using DulceRelax.App.Admin.Services;

namespace DulceRelax.App.Admin.Views;

public partial class PrincipalAdmin : ContentPage
{
    private readonly IAuthService _auth;

    public PrincipalAdmin(IAuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    private async void OnCerrarSesion(object sender, EventArgs e)
    {
        _auth.CerrarSesion();
        await Shell.Current.GoToAsync("..");
    }
}