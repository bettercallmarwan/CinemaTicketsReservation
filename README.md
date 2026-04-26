# Cinema Tickets Reservation

A backend API for reserving cinema seats. The main challenge this project solves is preventing double bookings when multiple users try to reserve the same seat at the same time.

## The Double Booking Problem

When two users click "reserve" on the same seat at the same moment, a naive implementation would let both requests read the seat as available, and both would succeed. One user ends up with a reservation that doesn't actually exist.

This project uses PostgreSQL row-level locking to solve it. When a reservation request comes in, the seat row is locked at the database level using `SELECT ... FOR UPDATE NOWAIT` inside a serializable transaction. If a second request hits the same seat while the first is still processing, PostgreSQL immediately rejects it with error code `55P03` (lock not available) instead of making it wait. The application catches this and returns a conflict response telling the user to try again. Deadlocks (`40P01`) are handled the same way.

The reservation flow works in three stages:

1. **Lock** -- A user reserves a seat. The seat status moves from `Free` to `Locked` and a reservation is created with status `Pending`. The reservation expires in 10 minutes.

2. **Pay** -- The user is redirected to a Stripe Checkout session. If payment succeeds, Stripe sends a webhook that moves the reservation to `Confirmed` and the seat to `Booked`. The webhook handler also acquires a `FOR UPDATE` lock on the reservation row before updating it, so it doesn't race with the cleanup service.

3. **Expire** -- A background service (`ReservationCleanupService`) runs every minute and finds all `Pending` reservations past their `ExpiresAt` time. It locks each one with `FOR UPDATE`, marks it `Expired`, and frees the seat. If the webhook already confirmed the reservation between the initial query and the lock acquisition, the service skips it.

This means a seat is never double-sold. The pessimistic lock guarantees that only one transaction can touch a given seat or reservation row at a time. The 10-minute expiry window prevents seats from being held indefinitely by users who never pay.

## API

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/Auth/register` | Create an account |
| `POST` | `/api/Auth/login` | Get a JWT token |
| `GET` | `/api/Movie` | List movies |
| `GET` | `/api/Movie/seats/{id}` | View seats and their availability for a movie |
| `POST` | `/api/Movie` | Create a movie with seats (admin) |
| `POST` | `/api/Reservation/reserve` | Reserve a seat (acquires row lock) |
| `POST` | `/api/Reservation/cancel` | Cancel and refund a confirmed reservation |
| `GET` | `/api/Reservation/me` | List your reservations |
| `POST` | `/api/Payment/create-checkout-session` | Start Stripe checkout for a pending reservation |
| `POST` | `/api/Payment/webhook` | Stripe webhook endpoint |

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core with PostgreSQL
- Stripe (payments, refunds)
- JWT authentication
- AutoMapper
