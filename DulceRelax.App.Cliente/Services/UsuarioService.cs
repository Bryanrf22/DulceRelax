using System.Net.Http.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO?> ObtenerMeAsync();
    }

    public class UsuarioService(HttpClient http) : IUsuarioService
    {
        public async Task<UsuarioDTO?> ObtenerMeAsync()
        {
            try { return await http.GetFromJsonAsync<UsuarioDTO>("api/usuario/me"); }
            catch { return null; }
        }
    }
}