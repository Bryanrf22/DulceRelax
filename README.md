# DulceRelax

DulceRelax es un sistema para agendar masajes a domicilio. Un cliente pide una cita
eligiendo masaje, fecha, hora y dirección, y un administrador la confirma, la reprograma
o la cancela desde su agenda. El sistema se compone de una API REST en ASP.NET Core y dos
aplicaciones .NET MAUI: una para el cliente y otra para el administrador.

## Características

App Cliente (`DulceRelax.App.Cliente`)

- Registro e inicio de sesión con correo y contraseña.
- Recuperación de contraseña por correo.
- Catálogo de masajes disponibles con precio, duración y descripción.
- Solicitud de cita con dirección exacta, teléfono y un detalle para ubicar la casa.
- Edición de citas pendientes y cancelación.
- Consulta de "Mis citas".
- Perfil del usuario, con edición de nombre, teléfono y dirección.

App Administrador (`DulceRelax.App.Admin`)

- Inicio de sesión restringido a cuentas con el rol de administrador.
- Agenda por día, con navegación al día anterior, al siguiente y un atajo a hoy.
- Confirmación o cancelación de solicitudes pendientes.
- Alta y edición de masajes, incluida la disponibilidad.
- Listado de clientes.
- Configuración del horario de atención por día de la semana.
- Trabajo sin conexión con las citas confirmadas que ya se descargaron en el equipo.
- Avisos por notificación cuando el cliente pide una cita o la cancela.
- Aviso cuando una solicitud sigue sin confirmar cerca de la fecha de la cita.

## Arquitectura

El repositorio es una solución de cuatro proyectos.

| Proyecto | Tipo | Responsabilidad |
| --- | --- | --- |
| `DulceRelax.API` | ASP.NET Core Web API (`net9.0`) | Expone los controladores REST y habla con Firebase. |
| `DulceRelax.Shared` | Biblioteca (`net9.0`) | DTOs compartidos entre la API y las dos apps. |
| `DulceRelax.App.Cliente` | .NET MAUI | Aplicación del cliente. |
| `DulceRelax.App.Admin` | .NET MAUI | Aplicación del administrador. |

Las apps MAUI no comparten interfaz: cada una reutiliza los DTOs de `DulceRelax.Shared`
y su propia capa de servicios (`Services/`), modelos de vista (`ViewModels/`) y vistas
(`Views/`).

El flujo de una petición pasa por una app, la API y Firestore. Firebase Authentication
emite los tokens, la API valida el JWT contra el proyecto de Firebase y Firestore guarda
los datos.

## Tecnologías

- .NET 9.0
- ASP.NET Core Web API con controladores
- .NET MAUI
- Firebase Authentication (tokens JWT)
- Cloud Firestore para la persistencia
- SQLite local en la app de administrador (caché y login sin conexión)
- CommunityToolkit.Mvvm en las dos apps
- Fuentes Material Icons y OpenSans

## Requisitos previos

- SDK de .NET 9.0 o posterior.
- Carga de trabajo de MAUI: `dotnet workload install maui`.
- Un proyecto de Firebase con Authentication por correo y contraseña habilitado, y una
  base de datos de Firestore.
- La clave de cuenta de servicio del proyecto de Firebase y su Web API key.
- Para compilar la app de Android, el SDK de Android. Para iOS y Mac Catalyst se necesita
  macOS con Xcode. El destino de Windows solo se agrega cuando se compila en Windows.

## Configuración

### API

`DulceRelax.API/appsettings.json` solo trae el nivel de log, `AllowedHosts` y la zona
horaria del negocio:

```json
"Negocio": { "ZonaHoraria": "America/Managua" }
```

Los valores de Firebase no se guardan en el archivo. La API los lee de la configuración
y, en desarrollo, conviene dejarlos con user secrets. El proyecto ya trae un
`UserSecretsId` en `DulceRelax.API.csproj`, así que basta con:

```bash
cd DulceRelax.API
dotnet user-secrets set "Firebase:ProjectId" "<tu-project-id>"
dotnet user-secrets set "Firebase:CredentialsJson" "$(cat /ruta/a/serviceAccountKey.json)"
dotnet user-secrets set "Firebase:WebApiKey" "<tu-web-api-key>"
```

Claves que lee la API:

| Clave | Uso |
| --- | --- |
| `Firebase:ProjectId` | Proyecto de Firebase, usado como emisor y audiencia del JWT. |
| `Firebase:CredentialsJson` | Contenido JSON de la cuenta de servicio para Firestore y Firebase Admin. |
| `Firebase:WebApiKey` | Web API key para iniciar sesión, recuperar contraseña y refrescar el token. |
| `Negocio:ZonaHoraria` | Zona horaria para validar el horario de atención. Por defecto, `America/Managua`. |

No subas credenciales al repositorio. El `.gitignore` ya excluye `serviceAccountKey.json`,
`firebase-adminsdk*.json`, `appsettings.Development.json`, `appsettings.Local.json`,
`google-services.json`, `GoogleService-Info.plist` y los archivos `*.keystore` y `*.jks`.

### Rol de administrador

La API autoriza el rol por una política llamada `Admin` que exige el claim `admin` con
valor `true` en el token. La app de administrador también comprueba ese claim antes de
dejar entrar. El claim se asigna en la cuenta de Firebase; el sistema no incluye ningún
endpoint que lo cree.

### Apps MAUI

La dirección base de la API está fijada en cada `MauiProgram.cs`:

```csharp
var apiUrl = new Uri("https://jkr8tzhd-7204.use.devtunnels.ms/");
```

Esa URL corresponde a un dev tunnel. Cámbiala por la dirección de tu API, por ejemplo
`https://localhost:7204/` o `http://localhost:5082/`, antes de ejecutar las apps.

### Firebase Cloud Messaging (app de administrador)

Para que la app de administrador reciba los avisos por push hay que registrar el
dispositivo en el proyecto de Firebase.

1. En la consola de Firebase, agregá una app de Android con el nombre de paquete de la
   app de administrador, que es `com.companyname.dulcerelax.app.admin` (el `ApplicationId`
   de `DulceRelax.App.Admin.csproj`).
2. Descargá el `google-services.json` de esa app y copialo en la raíz de
   `DulceRelax.App.Admin/`. El archivo no se versiona, así que cada quien lo baja de su
   propia cuenta.
3. Habilitá la API de Firebase Cloud Messaging en el proyecto. La cuenta de servicio que
   usa la API es la que envía los avisos.

El `.csproj` ya incluye el archivo como `GoogleServicesJson` y avisa con un warning de
compilación cuando no está presente:

```xml
<GoogleServicesJson Include="google-services.json"
  Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android' and Exists('google-services.json')" />
```

Sin ese archivo la app compila, pero Firebase no se inicializa y no llegan los avisos.

La app se registra sola en la API al iniciar sesión: `PushService` pide el token de FCM
y lo manda a `POST api/dispositivo`. Al cerrar sesión, el token se da de baja para que el
equipo deje de recibir avisos de administrador.

## Cómo ejecutar

API:

```bash
cd DulceRelax.API
dotnet run
```

El perfil `http` escucha en `http://localhost:5082` y el perfil `https` en
`https://localhost:7204;http://localhost:5082`. En entorno de desarrollo, la API expone
el documento OpenAPI.

Apps MAUI. Cada app apunta a varios marcos de destino, así que hay que indicar el
framework y, en Android, el emulador o dispositivo:

```bash
cd DulceRelax.App.Cliente
dotnet build -f net9.0-android

cd ../DulceRelax.App.Admin
dotnet build -f net9.0-android
```

## Endpoints de la API

Todos los controladores cuelgan de `/api`. Salvo los marcados como anónimos, cada endpoint
pide un token válido.

Usuario (`/api/usuario`)

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| POST | `/api/usuario/registrar` | Anónimo | Crea la cuenta en Firebase y su documento en Firestore. |
| POST | `/api/usuario/login` | Anónimo | Inicia sesión y devuelve `idToken` y `refreshToken`. |
| POST | `/api/usuario/recuperar-password` | Anónimo | Envía el correo de restablecimiento. |
| POST | `/api/usuario/refresh` | Anónimo | Renueva el `idToken` a partir del `refreshToken`. |
| GET | `/api/usuario` | Admin | Lista todos los usuarios. |
| GET | `/api/usuario/{id}` | Admin | Consulta un usuario por id. |
| GET | `/api/usuario/me` | Autenticado | Datos del usuario del token. |
| PUT | `/api/usuario/me` | Autenticado | Actualiza nombre, teléfono y dirección. |
| DELETE | `/api/usuario/me` | Autenticado | Borra la cuenta en Firebase y en Firestore. |

Cita (`/api/cita`)

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/cita/rango?desde&hasta` | Admin | Citas en un rango de hasta 7 días. |
| GET | `/api/cita/pendientes` | Admin | Solicitudes pendientes desde hoy. |
| GET | `/api/cita/agendadas?desde&hasta` | Admin | Citas confirmadas en un rango de hasta 3 días. |
| GET | `/api/cita?fecha` | Admin | Citas de un día. |
| GET | `/api/cita/disponibilidad?desde&hasta` | Autenticado | Franjas ocupadas por citas confirmadas. |
| GET | `/api/cita/mias` | Autenticado | Citas del usuario del token. |
| GET | `/api/cita/{id}` | Autenticado | Detalle de una cita. Solo si es propia o si el usuario es admin. |
| POST | `/api/cita` | Autenticado | Crea una solicitud de cita. |
| PUT | `/api/cita/{id}` | Autenticado | Actualiza una cita. |
| PATCH | `/api/cita/{id}/estado?estado` | Autenticado | Cambia el estado. El cliente solo puede cancelar. |

Masaje (`/api/masaje`)

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/masaje` | Autenticado | Lista los masajes. |
| GET | `/api/masaje/{id}` | Autenticado | Consulta un masaje por id. |
| POST | `/api/masaje/registrar` | Autenticado | Registra un masaje. |
| PUT | `/api/masaje/{id}` | Autenticado | Actualiza un masaje. |
| DELETE | `/api/masaje/{id}` | Autenticado | Elimina un masaje. |

Horario (`/api/horario`)

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/horario` | Autenticado | Devuelve el horario. Responde 204 si no hay. |
| PUT | `/api/horario` | Admin | Guarda el horario de los 7 días. |

Dispositivo (`/api/dispositivo`)

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| POST | `/api/dispositivo` | Admin | Registra el token de FCM del dispositivo. |
| POST | `/api/dispositivo/baja` | Admin | Quita el token de FCM del dispositivo. |

## Reglas de negocio

Las reglas viven en la API, en `CitaController` y `CitaRepository`.

- Una cita debe pedirse con al menos 5 días de anticipación. La validación se repite en
  el cliente para dar el aviso antes de enviar.
- Entre dos citas confirmadas debe quedar una hora de descanso. Al confirmar, la API
  rechaza con `409` si la cita se cruza con otra confirmada.
- La duración de la cita sale de los minutos del masaje. Si el masaje no trae duración,
  se usan 60 minutos.
- La hora solicitada debe caer dentro del horario de atención del día correspondiente,
  medido en la zona horaria del negocio.
- Los estados de una cita son `Pendiente`, `Confirmada` y `Cancelada`. El cliente solo
  edita citas pendientes y solo puede cancelar. El administrador cambia cualquier estado.
- El horario debe incluir los 7 días. Cada día activo tiene hora de inicio y de fin en
  formato `hh:mm`, y el inicio debe ser menor que el fin.

## Modelo de datos

Firestore guarda los datos en tres colecciones y un documento de configuración.

| Ruta | Campos |
| --- | --- |
| `usuarios/{uid}` | `NombreCompleto`, `NumTelefono`, `Correo`, `Direccion` |
| `citas/{id}` | `UsuarioId`, `UsuarioNombre`, `UsuarioTelefono`, `DireccionExacta`, `DetalleDireccion`, `MasajeId`, `MasajeNombre`, `DuracionMinutos`, `FechaHora`, `Estado` |
| `masajes/{id}` | `Nombre`, `Descripcion`, `Disponible`, `Precio`, `DuracionMinutos`, `ImagenUrl` |
| `configuracion/horario` | `Dias`: mapa de los 7 días, cada uno con `Activo`, `HoraInicio` y `HoraFin` |

El id del usuario es el `uid` que genera Firebase Authentication.

Las notificaciones agregan dos colecciones más:

| Ruta | Campos |
| --- | --- |
| `dispositivosAdmin/{token}` | `Actualizado`. El id del documento es el token de FCM, así que registrarlo dos veces no duplica. |
| `avisosLimite/{citaId}` | `Fecha`. Marca que esa cita ya recibió el aviso de fecha límite. |

## Notificaciones

El sistema tiene tres tipos de aviso, con mecanismos distintos.

| Aviso | Origen | Mecanismo | Funciona sin conexión |
| --- | --- | --- | --- |
| Recordatorio de cita confirmada | App de administrador | Notificación local programada 1 día y 1 hora antes | Sí |
| Nueva solicitud de cita | API | Push de FCM | No |
| Cita cancelada por el cliente | API | Push de FCM | No |
| Solicitud sin confirmar cerca de la fecha | API | Push de FCM desde un `BackgroundService` | No |

Los recordatorios se programan en el dispositivo, así que llegan aunque la app esté
cerrada y no haya red. Se recalculan en cada sincronización: `NotificationService`
cancela los avisos pendientes y vuelve a programar los de las citas confirmadas, de modo
que una cita cancelada deja de tener recordatorio.

Los avisos de push los manda la API con `PushSender`, que usa el SDK de administración
de Firebase. Necesitan internet porque el evento ocurre en el servidor.

El worker `AvisoLimiteWorker` revisa cada hora las citas pendientes y avisa cuando faltan
menos de siete días para la cita. Para que no se duplique el aviso de "nueva solicitud",
una cita creada dentro de ese umbral queda marcada en `avisosLimite` desde el mismo
momento en que se crea.

> [!IMPORTANT]
> `BackgroundService` solo corre si la API está encendida todo el tiempo. Si el hosting
> escala a cero, hay que exponer la revisión en un endpoint protegido y llamarlo desde un
> scheduler externo.

## Modo sin conexión

Solo la app de administrador funciona sin conexión. La lógica está en `Services/`.

- `LocalDb` abre una base SQLite en `dulcerelax_admin.db3` dentro de la carpeta de datos
  de la app y crea las tablas `citas`, `clientes` y `cuenta_offline`.
- `CitaCacheService` guarda las citas confirmadas y `ClienteCacheService` guarda los
  clientes. Ambos registran la última sincronización en `Preferences`.
- La agenda muestra la caché cuando la red falla y el día pedido está cubierto, y avisa
  que se están viendo datos guardados. La cobertura abarca hoy y mañana.
- `CuentaOfflineService` permite iniciar sesión sin red. La contraseña se guarda como
  hash PBKDF2-SHA256 con 600 000 iteraciones, sal de 16 bytes y hash de 32 bytes, y se
  compara en tiempo constante. Para entrar sin conexión, la cuenta de administrador debe
  haber iniciado sesión antes en ese equipo.
- `RedHandler` envuelve los errores de red y `Red` reconoce los estados `502`, `503` y
  `504` como servidor no disponible.

## Estructura del repositorio

```
DulceRelax/
├── DulceRelax.API/            API REST
│   ├── Controllers/           Usuario, Cita, Masaje, Horario
│   ├── Repositories/          Acceso a Firestore
│   └── Program.cs             Punto de entrada de la API
├── DulceRelax.Shared/
│   └── DTOs/                  DTOs y enums compartidos
├── DulceRelax.App.Cliente/    App MAUI del cliente
│   ├── Services/              Clientes HTTP de la API
│   ├── ViewModels/            Modelos de vista
│   └── Views/                 Vistas XAML
├── DulceRelax.App.Admin/      App MAUI del administrador
│   ├── Services/              Clientes HTTP, caché SQLite y sincronización
│   ├── ViewModels/
│   └── Views/
└── DulceRelax.sln
```

## Notas

- El punto de entrada de la API es `DulceRelax.API/Program.cs`. En la raíz del
  repositorio hay otro `Program.cs` que no forma parte de la solución.
- `WeatherForecastController` y `WeatherForecast.cs` en la API son restos de la plantilla
  de ASP.NET Core y no se usan.
- El proyecto no incluye pruebas automatizadas.

## Flujo de trabajo con Git

El repositorio trabaja por ramas. `master` recibe los cambios solo por medio de un pull
request, nunca con un push directo.

Para hacer un cambio:

```bash
git checkout master
git pull
git checkout -b feat/mi-cambio
# ... editar archivos ...
git add .
git commit -m "feat: descripción del cambio"
git push -u origin feat/mi-cambio
```

Luego se abre el pull request desde `feat/mi-cambio` hacia `master`. Los nombres de rama
que ya se usan en el repositorio empiezan por `feat/`, `fix/` o `feature/`.

## Créditos

Los iconos de la interfaz usan la fuente **Material Icons** de Google
(https://github.com/google/material-design-icons), distribuida bajo la
licencia Apache 2.0.
