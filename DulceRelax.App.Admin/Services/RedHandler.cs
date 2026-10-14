namespace DulceRelax.App.Admin.Services
{
    public class RedHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            try { return await base.SendAsync(request, ct); }
            catch (Exception ex) when (ex is not HttpRequestException and not TaskCanceledException)
            {
                throw new HttpRequestException(ex.Message, ex);
            }
        }
    }
}