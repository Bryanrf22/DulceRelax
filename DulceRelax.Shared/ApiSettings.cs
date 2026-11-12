namespace DulceRelax.Shared
{
    /// <summary>
    /// Dirección base de la API, compartida por las dos apps MAUI.
    /// En Debug apunta a la API local; en Release, a la API desplegada.
    /// </summary>
    public static class ApiSettings
    {
        public const string PreferenceKey = "api_base_url";

#if DEBUG
        // El emulador de Android llega al equipo anfitrión por 10.0.2.2, no por localhost.
        public const string BaseUrl = "http://10.0.2.2:5082/";
#else
        public const string BaseUrl = "https://dulcerelax.onrender.com/";
#endif

        /// <summary>
        /// Usa una dirección guardada en Preferences si es válida; si no, la de compilación.
        /// </summary>
        public static Uri Resolve(string? overrideUrl) =>
            Uri.TryCreate(overrideUrl, UriKind.Absolute, out var uri)
                ? uri
                : new Uri(BaseUrl);
    }
}
