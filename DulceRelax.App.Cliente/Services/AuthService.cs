using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public interface IAuthService
    {
        Task<string?> RecuperarPasswordAsync(string email);
        Task<string?> LoginAsync(LoginDTO dto);
        Task<string?> RegistrarAsync(RegistrarUsuarioDTO dto);
    }

    public class AuthService(HttpClient http) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            try
            {
                var res = await http.PostAsJsonAsync("api/usuario/login", dto);
                var body = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode) return await LeerError(res);

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                await SecureStorage.SetAsync("idToken", json.GetProperty("idToken").GetString()!);
                return null;
            }
            catch (HttpRequestException ex) { return $"Sin conexión: {ex.Message}"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        public async Task<string?> RegistrarAsync(RegistrarUsuarioDTO dto)
        {
            var res = await http.PostAsJsonAsync("api/usuario/registrar", dto);
            return res.IsSuccessStatusCode ? null : await LeerError(res);
        }

        public async Task<string?> RecuperarPasswordAsync(string email)
        {
            try
            {
                var res = await http.PostAsJsonAsync("api/usuario/recuperar-password",
                    new RecuperarPasswordDTO { Email = email });
                return res.IsSuccessStatusCode ? null : await LeerError(res);
            }
            catch (HttpRequestException) { return "No se pudo conectar con el servidor."; }
        }

        static async Task<string> LeerError(HttpResponseMessage res)
        {
            try
            {
                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                return json.GetProperty("error").GetString() ?? "Error desconocido";
            }
            catch { return "Error de conexion con el servidor"; }
        }
    }
}
