# Screenshots

This folder holds the images used by the READMEs. The files are not committed yet
because they must be captured from a running build with test data.

## Checklist

| File | Screen | Status |
| --- | --- | --- |
| `dulcerelax-client-catalog-en.webp` | Client: service catalog with price and duration | Pending |
| `dulcerelax-client-booking-en.webp` | Client: booking form with date, time and address | Pending |
| `dulcerelax-client-appointments-en.webp` | Client: "My appointments" with pending and confirmed states | Pending |
| `dulcerelax-admin-agenda-en.webp` | Admin: daily agenda with confirmed appointments | Pending |
| `dulcerelax-admin-pending-en.webp` | Admin: pending requests | Pending |
| `dulcerelax-flow.gif` | Booking → confirmation flow, 10–15 seconds | Pending |

## Rules

- Capture from the Android emulator or a test device.
- Use test data only: fictional names, emails, phones and addresses.
- Do not show the real business phone, customer names or addresses.
- Width between 1200 and 1600 px, WebP, under 300 KB each.
- Suggested tools: `flameshot` or `spectacle`, `peek` for the GIF, `cwebp` to convert.

## How to add them

1. Capture the screen and convert it to WebP:
   ```bash
   cwebp -q 80 screenshot.png -o docs/screenshots/dulcerelax-client-booking-en.webp
   ```
2. Drop the file in this folder using the exact name from the table.
3. Uncomment the matching image lines in `README.md` and `README.es.md`.
4. Commit:
   ```bash
   git add docs/screenshots
   git commit -m "docs: add client and admin screenshots"
   ```
