using DulceRelax.API.Repositories;
using FirebaseAdmin.Messaging;

namespace DulceRelax.API.Services
{
    public class PushSender(DispositivoRepository dispositivos, ILogger<PushSender> log)
    {
        public async Task EnviarAAdminsAsync(string titulo, string cuerpo)
        {
            try
            {
                var tokens = await dispositivos.GetTokensAsync();
                if (tokens.Count == 0) return;

                var resp = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(
                    new MulticastMessage
                    {
                        Tokens = tokens,
                        Notification = new Notification { Title = titulo, Body = cuerpo },
                        Android = new AndroidConfig { Priority = Priority.High }
                    });

                for (var i = 0; i < resp.Responses.Count; i++)
                {
                    var ex = resp.Responses[i].Exception;
                    if (ex?.MessagingErrorCode == MessagingErrorCode.Unregistered)
                        await dispositivos.EliminarAsync(tokens[i]);
                }
            }
            catch (Exception ex) { log.LogError(ex, "Error enviando push"); }
        }
    }
}