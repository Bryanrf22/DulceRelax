using System.Net;
using System.Net.Http.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.Services
{
    public interface IUsuarioService
    {
        Task<(List<UsuarioDTO> Lista, string? Error)> ObtenerTodosAsync();
    }

    public class UsuarioService(HttpClient http) : IUsuarioService
    {
        public async Task<(List<UsuarioDTO> Lista, string? Error)> ObtenerTodosAsync()
        {
            try
            {
                var res = await http.GetAsync("api/usuario");
                if (res.StatusCode == HttpStatusCode.Unauthorized) return ([], "Sesión expirada");
                if (res.StatusCode == HttpStatusCode.Forbidden) return ([], "No tienes permiso para ver los clientes");
                if (!res.IsSuccessStatusCode) return ([], "No se pudieron cargar los clientes");

                var lista = await res.Content.ReadFromJsonAsync<List<UsuarioDTO>>() ?? [];
                return (lista, null);
            }
            catch (HttpRequestException) { return ([], "Sin conexión con el servidor"); }
            catch (TaskCanceledException) { return ([], "Tiempo de espera agotado"); }
        }
    }
}