using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDTO dto, bool recordar = false);
        Task<bool> RestaurarSesionAsync();
        void CerrarSesion();
    }

    public class AuthService(HttpClient http, ICuentaOfflineService offline) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginDTO dto, bool recordar = false)
        {
            HttpResponseMessage res;
            try { res = await http.PostAsJsonAsync("api/usuario/login", dto); }
            catch (Exception) { return await LoginOfflineAsync(dto); }

            if (Red.NoDisponible(res.StatusCode)) return await LoginOfflineAsync(dto);
            if (!res.IsSuccessStatusCode) return await LeerError(res);

            try
            {
                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                if (!EsAdmin(json.GetProperty("idToken").GetString()!))
                    return "Esta cuenta no tiene permisos de administrador.";

                await GuardarTokensAsync(json, recordar);
            }
            catch (Exception) { return "Respuesta inesperada del servidor."; }

            try { await offline.GuardarAsync(dto.Email, dto.Password); } catch { }
            return null;
        }

        private async Task<string?> LoginOfflineAsync(LoginDTO dto)
        {
            try
            {
                return await offline.VerificarAsync(dto.Email, dto.Password) switch
                {
                    ResultadoOffline.Ok => null,
                    ResultadoOffline.Incorrecta => "Correo o contraseña incorrectos.",
                    _ => "Sin conexión con el servidor. Para entrar sin conexión, esta cuenta de " +
                         "administrador debe haber iniciado sesión antes en este dispositivo."
                };
            }
            catch { return "No se pudo verificar la cuenta sin conexión."; }
        }

        public async Task<bool> RestaurarSesionAsync()
        {
            try
            {
                var refresh = await SecureStorage.GetAsync("refreshToken");
                if (string.IsNullOrEmpty(refresh)) return false;

                var res = await http.PostAsJsonAsync("api/usuario/refresh",
                    new RefreshTokenDTO { RefreshToken = refresh });

                if (Red.NoDisponible(res.StatusCode)) return true;

                if (!res.IsSuccessStatusCode)
                {
                    if (res.StatusCode == HttpStatusCode.Unauthorized) CerrarSesion();
                    return false;
                }

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                if (!EsAdmin(json.GetProperty("idToken").GetString()!))
                {
                    CerrarSesion();
                    return false;
                }

                await GuardarTokensAsync(json, recordar: true);
                return true;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return true;
            }
            catch { return false; }
        }

        public void CerrarSesion()
        {
            SecureStorage.Remove("idToken");
            SecureStorage.Remove("refreshToken");
        }

        static bool EsAdmin(string jwt)
        {
            try
            {
                var p = jwt.Split('.')[1].Replace('-', '+').Replace('_', '/');
                p = p.PadRight(p.Length + (4 - p.Length % 4) % 4, '=');
                using var doc = JsonDocument.Parse(Convert.FromBase64String(p));
                return doc.RootElement.TryGetProperty("admin", out var a)
                    && (a.ValueKind == JsonValueKind.True
                        || (a.ValueKind == JsonValueKind.String
                            && a.GetString()!.Equals("true", StringComparison.OrdinalIgnoreCase)));
            }
            catch { return false; }
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