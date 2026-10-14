using CommunityToolkit.Mvvm.Input;

namespace DulceRelax.App.Admin.Services
{
    public static class CommandExtensions
    {
        public static async Task EjecutarSeguroAsync(this IAsyncRelayCommand comando)
        {
            try { await comando.ExecuteAsync(null); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
        }
    }
}