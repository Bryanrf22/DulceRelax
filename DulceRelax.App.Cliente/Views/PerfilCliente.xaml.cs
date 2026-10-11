using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class PerfilCliente : ContentPage
{
    private readonly PerfilViewModel _vm;

    public PerfilCliente(PerfilViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    #if ANDROID
        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window?
            .SetFlags(Android.Views.WindowManagerFlags.Secure, Android.Views.WindowManagerFlags.Secure);
    #endif
        await _vm.CargarCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    #if ANDROID
        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window?
            .ClearFlags(Android.Views.WindowManagerFlags.Secure);
    #endif
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}