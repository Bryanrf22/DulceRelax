using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DulceRelax.App.Cliente.Services
{
    public class AuthHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var token = await SecureStorage.GetAsync("idToken");

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new("Bearer", token);
            return await base.SendAsync(request, ct);
        }
    }
}
