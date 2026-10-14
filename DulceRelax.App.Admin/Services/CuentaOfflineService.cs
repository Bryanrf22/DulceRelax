using System.Security.Cryptography;
using SQLite;

namespace DulceRelax.App.Admin.Services
{
    [Table("cuenta_offline")]
    public class CuentaOffline
    {
        [PrimaryKey] public string Correo { get; set; } = "";
        public string Salt { get; set; } = "";
        public string Hash { get; set; } = "";
    }

    public enum ResultadoOffline { SinRegistro, Incorrecta, Ok }

    public interface ICuentaOfflineService
    {
        Task GuardarAsync(string correo, string password);
        Task<ResultadoOffline> VerificarAsync(string correo, string password);
    }

    public class CuentaOfflineService(LocalDb local) : ICuentaOfflineService
    {
        private const int Iteraciones = 600_000;

        public async Task GuardarAsync(string correo, string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = await Task.Run(() => Derivar(password, salt));

            var db = await local.GetAsync();
            await db.InsertOrReplaceAsync(new CuentaOffline
            {
                Correo = Normalizar(correo),
                Salt = Convert.ToBase64String(salt),
                Hash = Convert.ToBase64String(hash)
            });
        }

        public async Task<ResultadoOffline> VerificarAsync(string correo, string password)
        {
            var db = await local.GetAsync();
            var cuenta = await db.FindAsync<CuentaOffline>(Normalizar(correo));
            if (cuenta is null) return ResultadoOffline.SinRegistro;

            var esperado = Convert.FromBase64String(cuenta.Hash);
            var salt = Convert.FromBase64String(cuenta.Salt);
            var calculado = await Task.Run(() => Derivar(password, salt));

            return CryptographicOperations.FixedTimeEquals(esperado, calculado)
                ? ResultadoOffline.Ok
                : ResultadoOffline.Incorrecta;
        }

        private static byte[] Derivar(string password, byte[] salt) =>
            Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, 32);

        private static string Normalizar(string correo) => correo.Trim().ToLowerInvariant();
    }
}