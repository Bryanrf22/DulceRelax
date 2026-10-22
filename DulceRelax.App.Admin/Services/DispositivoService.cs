using System.Net.Http.Json;

namespace DulceRelax.App.Admin.Services
{
    public interface IDispositivoService
    {
        Task RegistrarAsync(string token);
        Task QuitarAsync(string token);
    }

    public class DispositivoService(HttpClient http) : IDispositivoService
    {
        public Task RegistrarAsync(string token) =>
            http.PostAsJsonAsync("api/dispositivo", new { Token = token });

        public Task QuitarAsync(string token) =>
            http.PostAsJsonAsync("api/dispositivo/baja", new { Token = token });
    }
}
