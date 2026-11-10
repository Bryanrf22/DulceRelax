# API reference

Base URL:

- Local: `http://localhost:5082/`
- Deployed: `https://dulcerelax.onrender.com/`

All controller routes are under `/api`. Except for the anonymous endpoints listed
below, every request needs a Firebase ID token:

```http
Authorization: Bearer <firebase-id-token>
```

The API validates the token against the Firebase project configured in
`Firebase:ProjectId`. Admin endpoints also require the custom claim `admin=true`.

Errors use a JSON body with an `error` field:

```json
{ "error": "La cita debe solicitarse con al menos 5 días de anticipación." }
```

| Status | Meaning |
| --- | --- |
| `200` | Success with a body |
| `204` | Success without a body |
| `400` | Validation error or business rule violation |
| `401` | Missing, invalid or expired token |
| `403` | Authenticated, but not allowed (not the owner or not an admin) |
| `404` | Resource not found |
| `409` | Time slot already taken |
| `502` | Firebase could not send the password recovery email |

## Users — `/api/usuario`

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| POST | `/api/usuario/registrar` | Anonymous | Creates the Firebase account and the Firestore document. Returns `{ uid }`. |
| POST | `/api/usuario/login` | Anonymous | Signs in and returns `{ idToken, refreshToken }`. |
| POST | `/api/usuario/recuperar-password` | Anonymous | Sends the password reset email. |
| POST | `/api/usuario/refresh` | Anonymous | Exchanges `refreshToken` for a new `idToken` and `refreshToken`. |
| GET | `/api/usuario` | Admin | Lists every user. |
| GET | `/api/usuario/{id}` | Admin | Gets one user by id. |
| GET | `/api/usuario/me` | Authenticated | Gets the user from the token. |
| PUT | `/api/usuario/me` | Authenticated | Updates name, phone and address. |
| DELETE | `/api/usuario/me` | Authenticated | Deletes the Firestore document and the Firebase account. |

## Appointments — `/api/cita`

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| GET | `/api/cita/rango?desde&hasta` | Admin | Appointments in a range of up to 7 days. |
| GET | `/api/cita/pendientes` | Admin | Pending requests from today onward. |
| GET | `/api/cita/agendadas?desde&hasta` | Admin | Confirmed appointments in a range of up to 3 days. |
| GET | `/api/cita?fecha` | Admin | Appointments for one day. |
| GET | `/api/cita/disponibilidad?desde&hasta` | Authenticated | Busy slots from confirmed appointments, in a range of up to 7 days. |
| GET | `/api/cita/mias` | Authenticated | Appointments that belong to the token user. |
| GET | `/api/cita/{id}` | Authenticated | Appointment detail. Owner or admin only. |
| POST | `/api/cita` | Authenticated | Creates a pending appointment request. |
| PUT | `/api/cita/{id}` | Authenticated | Updates an appointment. Clients can only edit pending ones. |
| PATCH | `/api/cita/{id}/estado?estado` | Authenticated | Changes the status. Clients can only cancel. |
| PATCH | `/api/cita/{id}/reprogramar` | Admin | Reschedules a confirmed appointment. |

`estado` accepts `Pendiente`, `Confirmada` or `Cancelada`.

## Services — `/api/masaje`

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| GET | `/api/masaje` | Authenticated | Lists services. |
| GET | `/api/masaje/{id}` | Authenticated | Gets one service by id. |
| POST | `/api/masaje/registrar` | Admin | Creates a service. |
| PUT | `/api/masaje/{id}` | Admin | Updates a service. |
| DELETE | `/api/masaje/{id}` | Admin | Deletes a service. |

## Business hours — `/api/horario`

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| GET | `/api/horario` | Authenticated | Returns the weekly schedule, or `204` when it is not configured. |
| PUT | `/api/horario` | Admin | Saves all seven days. |

The request body must contain all seven day names (`Monday` … `Sunday`). Each
active day needs `HoraInicio` and `HoraFin` in `hh:mm` format, and start must be
before end.

## Devices — `/api/dispositivo`

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| POST | `/api/dispositivo` | Admin | Registers an FCM token: `{ "token": "..." }`. |
| POST | `/api/dispositivo/baja` | Admin | Removes an FCM token: `{ "token": "..." }`. |

## Business rules

The rules are implemented in `CitaRules` (`DulceRelax.Shared`) and enforced by the
API:

- A booking needs at least 5 days of notice.
- Confirmed appointments keep a one-hour rest block around them.
- The appointment must fit inside the business hours of that weekday, measured in
  the business time zone (`Negocio:ZonaHoraria`).
- The duration comes from the service. When it is missing or zero, it is 60 minutes.
- Clients can only edit pending appointments and can only cancel.

## OpenAPI

In Development the API exposes the OpenAPI document at `/openapi/v1.json`. Run the
API locally and use the document with your preferred client.
