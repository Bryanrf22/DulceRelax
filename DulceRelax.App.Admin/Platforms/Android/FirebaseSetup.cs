using Android.App;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.Core.Platforms.Android;

namespace DulceRelax.App.Admin
{
    /// <summary>
    /// Arranque de Firebase en Android. Solo se compila para el destino de Android.
    /// </summary>
    public static class FirebaseSetup
    {
        public static void Initialize(Activity activity)
        {
            // Crea la app de Firebase por defecto a partir del google-services.json.
            CrossFirebase.Initialize(activity,
                () => Microsoft.Maui.ApplicationModel.Platform.CurrentActivity!);

            // Con la app en primer plano la notificación la muestra la propia app
            // (Plugin.LocalNotification), porque además tiene que sincronizar para
            // cancelar los recordatorios de una cita cancelada. Se anula la que
            // muestra el plugin para no mostrar dos avisos.
            FirebaseCloudMessagingImplementation.ShowLocalNotificationAction = _ => { };
        }
    }
}
