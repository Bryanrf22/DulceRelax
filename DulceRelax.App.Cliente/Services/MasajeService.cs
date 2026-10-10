using System.Net;
using System.Net.Http.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public interface IMasajeService
    {
        Task<(List<MasajeDTO> Lista, string? Error)> ObtenerDisponiblesAsync();
    }

    public class MasajeService(HttpClient http) : IMasajeService
    {
        public async Task<(List<MasajeDTO> Lista, string? Error)> ObtenerDisponiblesAsync()
        {
            try
            {
                var res = await http.GetAsync("api/masaje");
                if (!res.IsSuccessStatusCode)
                    return ([], res.StatusCode == HttpStatusCode.Unauthorized
                        ? "Sesión expirada"
                        : "No se pudieron cargar los masajes");

                var lista = await res.Content.ReadFromJsonAsync<List<MasajeDTO>>() ?? [];
                return (lista.Where(m => m.Disponible).ToList(), null);
            }
            catch (HttpRequestException) { return ([], "Sin conexión con el servidor"); }
            catch (TaskCanceledException) { return ([], "Tiempo de espera agotado"); }
        }
    }
}