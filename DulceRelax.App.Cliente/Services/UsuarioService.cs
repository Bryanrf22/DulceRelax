using System.Net;
using System.Net.Http.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO?> ObtenerMeAsync();
        Task<string?> ActualizarMeAsync(ActualizarUsuarioDTO dto);
    }

    public class UsuarioService(HttpClient http) : IUsuarioService
    {
        public async Task<UsuarioDTO?> ObtenerMeAsync()
        {
            try { return await http.GetFromJsonAsync<UsuarioDTO>("api/usuario/me"); }
            catch { return null; }
        }

        public async Task<string?> ActualizarMeAsync(ActualizarUsuarioDTO dto)
        {
            try
            {
                var res = await http.PutAsJsonAsync("api/usuario/me", dto);
                if (res.IsSuccessStatusCode) return null;

                return res.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Sesión expirada",
                    HttpStatusCode.BadRequest => "Revisa los datos ingresados",
                    _ => "No se pudo guardar el perfil"
                };
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }
    }
}