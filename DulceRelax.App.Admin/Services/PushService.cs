using Plugin.Firebase.CloudMessaging;
using Plugin.LocalNotification;

namespace DulceRelax.App.Admin.Services
{
    /// <summary>
    /// Registra el token FCM del dispositivo en la API para que los administradores
    /// reciban los avisos de nuevas solicitudes y cancelaciones.
    /// </summary>
    public class PushService(
        IFirebaseCloudMessaging? fcm,
        IDispositivoService dispositivos,
        ISincronizadorService sync)
    {
        private bool _suscrito;

        /// <summary>
        /// Pide el token de FCM y lo registra en la API. También se suscribe a los
        /// avisos para mostrarlos localmente mientras la app está abierta.
        /// </summary>
        public async Task IniciarAsync()
        {
            // En plataformas sin soporte de Firebase (Windows, MacCatalyst) no hay nada que hacer.
            if (fcm is null) return;

            try
            {
                await fcm.CheckIfValidAsync();

                if (!_suscrito)
                {
                    _suscrito = true;

                    fcm.TokenChanged += async (_, e) => await RegistrarAsync(e.Token);

                    // Con la app en primer plano Android no muestra la notificación del push,
                    // por eso se muestra localmente y se sincroniza (así se cancelan los
                    // recordatorios de una cita cancelada).
                    fcm.NotificationReceived += async (_, e) =>
                    {
                        try
                        {
                            await LocalNotificationCenter.Current.Show(new NotificationRequest
                            {
                                NotificationId = Random.Shared.Next(1, int.MaxValue),
                                Title = e.Notification.Title,
                                Description = e.Notification.Body
                            });

                            await sync.SincronizarAsync();
                        }
                        catch { }
                    };
                }

                await RegistrarAsync(await fcm.GetTokenAsync());
            }
            catch { }
        }

        /// <summary>
        /// Quita el token de este dispositivo de la API para que deje de recibir avisos.
        /// </summary>
        public async Task DetenerAsync()
        {
            if (fcm is null) return;

            try { await dispositivos.QuitarAsync(await fcm.GetTokenAsync()); }
            catch { }
        }

        private async Task RegistrarAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            try { await dispositivos.RegistrarAsync(token); }
            catch { }
        }
    }
}
