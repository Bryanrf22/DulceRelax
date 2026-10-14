using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.Services
{
    public interface IHorarioService
    {
        Task<(HorarioAtencionDTO? Horario, string? Error)> ObtenerAsync();
        Task<string?> GuardarAsync(HorarioAtencionDTO horario);
    }

    public class HorarioService(HttpClient http) : IHorarioService
    {
        public async Task<(HorarioAtencionDTO? Horario, string? Error)> ObtenerAsync()
        {
            try
            {
                var res = await http.GetAsync("api/horario");
                if (res.StatusCode == HttpStatusCode.NoContent) return (null, null);
                if (res.StatusCode == HttpStatusCode.Unauthorized) return (null, "Sesión expirada");
                if (!res.IsSuccessStatusCode) return (null, "No se pudo cargar el horario");

                return (await res.Content.ReadFromJsonAsync<HorarioAtencionDTO>(), null);
            }
            catch (HttpRequestException) { return (null, "Sin conexión con el servidor"); }
            catch (TaskCanceledException) { return (null, "Tiempo de espera agotado"); }
        }

        public async Task<string?> GuardarAsync(HorarioAtencionDTO horario)
        {
            try
            {
                var res = await http.PutAsJsonAsync("api/horario", horario);
                if (res.IsSuccessStatusCode) return null;

                return res.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Sesión expirada",
                    HttpStatusCode.Forbidden => "No tienes permiso para hacer esto",
                    _ => await LeerError(res)
                };
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        static async Task<string> LeerError(HttpResponseMessage res)
        {
            try
            {
                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                return json.GetProperty("error").GetString() ?? "No se pudo guardar el horario";
            }
            catch { return "No se pudo guardar el horario"; }
        }
    }
}