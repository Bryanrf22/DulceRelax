namespace DulceRelax.App.Admin.Services
{
    public interface ISincronizadorService
    {
        Task SincronizarAsync();
    }

    public class SincronizadorService(
        ICitaService citas, IUsuarioService usuarios,
        ICitaCacheService citaCache, IClienteCacheService clienteCache) : ISincronizadorService
    {
        private static readonly SemaphoreSlim _lock = new(1, 1);

        public async Task SincronizarAsync()
        {
            if (!await _lock.WaitAsync(0)) return;
            try
            {
                var (agendadas, e1) = await citas.ObtenerAgendadasAsync();
                if (e1 is null) await citaCache.GuardarAsync(agendadas);

                var (lista, e2) = await usuarios.ObtenerTodosAsync();
                if (e2 is null) await clienteCache.GuardarAsync(lista);
            }
            catch { }
            finally { _lock.Release(); }
        }
    }
}