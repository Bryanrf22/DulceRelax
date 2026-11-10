# Deployment

The API ships with a `Dockerfile` that restores only `DulceRelax.API` and
`DulceRelax.Shared`, so the MAUI projects are not part of the image.

## Environment variables

| Variable | Required | Description |
| --- | --- | --- |
| `Firebase__ProjectId` | Yes | Firebase project id. Used as JWT issuer and audience. |
| `Firebase__CredentialsJson` | Yes | Full JSON of the Firebase service account. |
| `Firebase__WebApiKey` | Yes | Web API key for sign-in, password recovery and token refresh. |
| `Negocio__ZonaHoraria` | No | Business time zone. Defaults to `America/Managua`. |

Never commit the service account JSON. On Render and Fly, store it as a secret.

## Render

1. Create a **Web Service** from this repository.
2. Runtime: **Docker**.
3. Add the environment variables above. For `Firebase__CredentialsJson`, paste the
   JSON as a single-line secret.
4. Deploy. The container listens on port `8080` (`ASPNETCORE_URLS`).
5. Set the public URL as the release base URL in `DulceRelax.Shared/ApiSettings.cs`
   if it differs from `https://dulcerelax.onrender.com/`.

## Fly.io

```bash
fly launch --no-deploy
fly secrets set \
  Firebase__ProjectId="<project-id>" \
  Firebase__CredentialsJson="$(cat serviceAccountKey.json)" \
  Firebase__WebApiKey="<web-api-key>" \
  Negocio__ZonaHoraria="America/Managua"
fly deploy
```

## Cold starts

Free hosting tiers can scale to zero. Two consequences:

- The first request after idle time takes a few seconds.
- `AvisoLimiteWorker` (the pending-request reminder) only runs while the API is
  awake.

If the API must stay available, use a paid instance or move the reminder to an
external scheduler that calls a protected endpoint.

## Health check

The API has no dedicated health endpoint. A request to any protected route should
return `401` when the process is up:

```bash
curl -i https://dulcerelax.onrender.com/api/masaje
```
