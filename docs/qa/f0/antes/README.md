# F0-T3 · Capturas «antes» de las 15 pantallas

**Tarea:** F0-T3
**Estado:** **Bloqueado por el entorno** (ver «Motivos»).
**Fecha del intento:** 22 de octubre de 2026.

## Protocolo de captura

1. Ejecutar la app contra la API local o el dev tunnel configurado en `MauiProgram.cs`.
2. Recorrer las 15 pantallas de la tabla.
3. Guardar cada captura como `NN-app-pantalla-plataforma.png`.
4. Repetir en Android y en iOS (o Mac Catalyst). Nombrar la plataforma como `android`, `ios` o `maccatalyst`.
5. Repetir con el tema oscuro del sistema **solo como evidencia** de DR-030 (el cliente fuerza claro; el admin no).
6. No recortar la pantalla: debe verse el sistema completo (barras incluidas).

Nombres esperados (ejemplo para Android):

```
01-cliente-login-android.png
02-cliente-registro-android.png
03-cliente-recuperar-android.png
04-cliente-inicio-android.png
05-cliente-reserva-android.png
06-cliente-mis-citas-android.png
07-cliente-perfil-android.png
08-cliente-editar-perfil-android.png
09-admin-login-android.png
10-admin-agenda-android.png
11-admin-solicitudes-android.png
12-admin-masajes-android.png
13-admin-form-masaje-android.png
14-admin-clientes-android.png
15-admin-ajustes-android.png
```

## Inventario

| # | App | Pantalla | Ruta XAML | Captura Android | Captura iOS |
|---|---|---|---|---|---|
| 1 | Cliente | Inicio de sesión | `Views/InicioSesionCliente.xaml` | Pendiente | Pendiente |
| 2 | Cliente | Registro | `Views/RegistroCliente.xaml` | Pendiente | Pendiente |
| 3 | Cliente | Recuperar contraseña | `Views/RecuperarContraseñaCliente.xaml` | Pendiente | Pendiente |
| 4 | Cliente | Catálogo / inicio | `Views/PrincipalCliente.xaml` | Pendiente | Pendiente |
| 5 | Cliente | Reservar cita | `Views/ReservaMasajeCliente.xaml` | Pendiente | Pendiente |
| 6 | Cliente | Mis citas | `Views/MisCitasCliente.xaml` | Pendiente | Pendiente |
| 7 | Cliente | Perfil | `Views/PerfilCliente.xaml` | Pendiente | Pendiente |
| 8 | Cliente | Editar perfil | `Views/PerfilClienteEdit.xaml` | Pendiente | Pendiente |
| 9 | Admin | Inicio de sesión | `Views/InicioSesionAdmin.xaml` | Pendiente | Pendiente |
| 10 | Admin | Agenda | `Views/PrincipalAdmin.xaml` | Pendiente | Pendiente |
| 11 | Admin | Solicitudes | `Views/SolicitudesAdmin.xaml` | Pendiente | Pendiente |
| 12 | Admin | Masajes | `Views/MasajesAdmin.xaml` | Pendiente | Pendiente |
| 13 | Admin | Formulario de masaje | `Views/MasajeFormAdmin.xaml` | Pendiente | Pendiente |
| 14 | Admin | Clientes | `Views/ClientesAdmin.xaml` | Pendiente | Pendiente |
| 15 | Admin | Ajustes | `Views/AjustesAdmin.xaml` | Pendiente | Pendiente |

## Motivos del bloqueo

| # | Bloqueo | Evidencia | Cómo se desbloquea |
|---|---|---|---|
| 1 | No hay equipo macOS ni simulador iOS. | `uname -s` = Linux; no existe `xcrun`. | Ejecutar la captura iOS/Mac Catalyst en un Mac con Xcode. |
| 2 | El dispositivo Android conectado está bloqueado con PIN. | `dumpsys window` muestra `NotificationShade` y `isKeyguardShowing=true`; los gestos de desbloqueo no avanzan. | Desbloquear el teléfono manualmente y mantenerlo desbloqueado durante la captura. |
| 3 | La URL de la API configurada no responde. | `curl https://jkr8tzhd-7204.use.devtunnels.ms/` devuelve `000` (sin conexión). | Levantar la API local y apuntar temporalmente `apiUrl` a `http://10.0.2.2:5082/` (emulador) o a la IP de la LAN (dispositivo físico). |
| 4 | La API no tiene secretos de Firebase configurados. | `dotnet user-secrets list` → «No secrets configured for this application». | Configurar `Firebase:ProjectId`, `Firebase:CredentialsJson` y `Firebase:WebApiKey` (ver README), y usar una cuenta de siembra cliente/admin. |
| 5 | No hay credenciales de siembra documentadas. | El README no incluye usuario/clave de prueba. | Crear un usuario cliente y uno administrador en Firebase, o documentar los de siembra en un archivo fuera del repositorio. |

## Intento realizado

1. Se compiló la app de cliente para Android (`0 errores`).
2. Se compiló una variante con `ApplicationId=com.companyname.dulcerelax.app.cliente.qa` para no desinstalar la app existente del dispositivo.
3. La variante se instaló correctamente, pero el dispositivo seguía en la pantalla de bloqueo; no fue posible llevar la app a primer plano.
4. La variante se desinstaló para no dejar residuos en el equipo.

> **No se inventaron capturas.** Las celdas «Pendiente» deben completarse antes de comparar el antes/después en Fase 10.

## Criterio de aceptación

- [ ] 15 capturas en Android.
- [ ] 15 capturas en iOS (o Mac Catalyst).
- [ ] Cada captura muestra la pantalla completa, sin recortes.
