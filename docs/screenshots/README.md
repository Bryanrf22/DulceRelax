# Screenshots

Captured from a debug build on an Android device (1080 × 2400) and converted to WebP.
All images use fictional test data.

| File | Screen |
| --- | --- |
| `dulcerelax-client-catalog.webp` | Client: service catalog with price and duration |
| `dulcerelax-client-booking.webp` | Client: booking form with date, time and address |
| `dulcerelax-client-appointments.webp` | Client: "My appointments" with confirmed and pending states |
| `dulcerelax-admin-agenda.webp` | Admin: daily agenda with a confirmed appointment |
| `dulcerelax-flow.gif` | Booking → confirmation flow (11 s) |

## Rules

- Fictional data only: names, emails, phones and addresses.
- Never include real customers, real business contact details, credentials or the
  contents of `google-services.json`.
- Keep the status bar: it shows the app running on a real device, not a mockup.
- WebP at 1080 px wide and under 100 KB each; the GIF stays under 300 KB.
- The apps are Spanish-only, so one set of images serves both READMEs.

## Not included

- **Admin pending requests.** The available capture contained data that was not clearly
  fictional, so it was discarded. Recapture it with a fictional request before adding it.

## Replacing or adding a screenshot

1. Capture the screen on the device or emulator with test data.
2. Convert it:
   ```bash
   cwebp -q 80 screenshot.png -o docs/screenshots/dulcerelax-client-booking.webp
   ```
3. Add or update the row above and reference the file from both READMEs.
4. Check `git status` before committing: raw PNGs, `uiautomator` dumps and conversion
   scripts do not belong in the repository.
