using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public class AuthHandler(IHttpClientFactory factory) : DelegatingHandler
    {
        private static readonly SemaphoreSlim _refrescando = new(1, 1);

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var cuerpo = request.Content is null ? null : await request.Content.ReadAsByteArrayAsync(ct);
            var tipo = request.Content?.Headers.ContentType;

            var token = await SecureStorage.GetAsync("idToken");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new("Bearer", token);

            var res = await base.SendAsync(request, ct);

            if (res.StatusCode != HttpStatusCode.Unauthorized
                || string.IsNullOrEmpty(token)
                || request.RequestUri?.AbsolutePath.EndsWith("/login") == true)
                return res;

            var nuevo = await RenovarAsync(token, ct);
            if (nuevo is null) return res;

            res.Dispose();

            var copia = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var h in request.Headers)
                copia.Headers.TryAddWithoutValidation(h.Key, h.Value);
            copia.Headers.Authorization = new("Bearer", nuevo);

            if (cuerpo is not null)
            {
                copia.Content = new ByteArrayContent(cuerpo);
                copia.Content.Headers.ContentType = tipo;
            }

            return await base.SendAsync(copia, ct);
        }

        private async Task<string?> RenovarAsync(string tokenFallido, CancellationToken ct)
        {
            await _refrescando.WaitAsync(ct);
            try
            {
                var actual = await SecureStorage.GetAsync("idToken");
                if (!string.IsNullOrEmpty(actual) && actual != tokenFallido) return actual;

                var refresh = await SecureStorage.GetAsync("refreshToken");
                if (string.IsNullOrEmpty(refresh)) return null;

                var http = factory.CreateClient("refresh");
                var res = await http.PostAsJsonAsync("api/usuario/refresh",
                    new RefreshTokenDTO { RefreshToken = refresh }, ct);
                if (!res.IsSuccessStatusCode) return null;

                var json = await res.Content.ReadFromJsonAsync<JsonElement>(ct);
                var id = json.GetProperty("idToken").GetString()!;
                await SecureStorage.SetAsync("idToken", id);
                await SecureStorage.SetAsync("refreshToken", json.GetProperty("refreshToken").GetString()!);
                return id;
            }
            catch { return null; }
            finally { _refrescando.Release(); }
        }
    }
}