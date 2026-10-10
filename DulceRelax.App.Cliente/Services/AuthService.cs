using DulceRelax.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DulceRelax.App.Cliente.Services
{
    public interface IAuthService
    {
        Task<string?> RecuperarPasswordAsync(string email);
        Task<string?> LoginAsync(LoginDTO dto, bool recordar = false);
        Task<bool> RestaurarSesionAsync();
        void CerrarSesion();
        Task<string?> RegistrarAsync(RegistrarUsuarioDTO dto);
    }

    public class AuthService(HttpClient http) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginDTO dto, bool recordar = false)
        {
            try
            {
                var res = await http.PostAsJsonAsync("api/usuario/login", dto);
                if (!res.IsSuccessStatusCode) return await LeerError(res);

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                await GuardarTokensAsync(json, recordar);
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

        public async Task<bool> RestaurarSesionAsync()
        {
            try
            {
                var refresh = await SecureStorage.GetAsync("refreshToken");
                if (string.IsNullOrEmpty(refresh)) return false;

                var res = await http.PostAsJsonAsync("api/usuario/refresh",
                    new RefreshTokenDTO { RefreshToken = refresh });

                if (!res.IsSuccessStatusCode)
                {
                    if (res.StatusCode == HttpStatusCode.Unauthorized) CerrarSesion();
                    return false;
                }

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                await GuardarTokensAsync(json, recordar: true);
                return true;
            }
            catch { return false; }
        }

        public void CerrarSesion()
        {
            SecureStorage.Remove("idToken");
            SecureStorage.Remove("refreshToken");
        }

        static async Task GuardarTokensAsync(JsonElement json, bool recordar)
        {
            await SecureStorage.SetAsync("idToken", json.GetProperty("idToken").GetString()!);
            if (recordar)
                await SecureStorage.SetAsync("refreshToken", json.GetProperty("refreshToken").GetString()!);
            else
                SecureStorage.Remove("refreshToken");
        }
    }
}
