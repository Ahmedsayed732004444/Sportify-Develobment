# Sportify API Status Audit

This document inventories all endpoints exposed by the ASP.NET Core backend and confirms their frontend integration status.

---

## 1. Authentication & Profile Endpoints
- `POST /auth/register` — User signup. **Connected**
- `POST /auth/login` — Token retrieval and JWT issuance. **Connected**
- `GET /profiles/{userId}` — Retrieves user stats, bio, and favorite sports. **Connected**
- `PUT /profiles/me/info` — Updates player name, bio, and cities. **Connected**

---

## 2. Onboarding & Subscriptions Endpoints
- `POST /membership-upgrades` — Wizard request submission for new clubs. **Connected**
- `GET /me/membership-request` — Check onboarding status (Pending/Approved). **Connected**
- `GET /subscription-plans` — Get active plans list. **Connected**
- `GET /clubs/{clubId}/subscriptions/requests` — Log subscription renewal/upgrade history. **Connected**
- `POST /clubs/{clubId}/subscriptions/requests` — Submit renewal/upgrade request. **Connected**
- `GET /admin/subscription-requests` — Admin queue for requests review. **Connected**
- `POST /admin/subscription-requests/{id}/approve` — Approve renewal/upgrade. **Connected**
- `POST /admin/subscription-requests/{id}/reject` — Reject renewal/upgrade. **Connected**

---

## 3. Venue & Court Management Endpoints
- `GET /clubs` — Explore registered clubs. **Connected**
- `GET /clubs/{clubId}/courts` — List courts in a sports center (accessible by guests; shows inactive courts to owners). **Connected**
- `POST /clubs/{clubId}/courts` — Add pitch (Multipart Form Data with photo upload). **Connected**
- `PUT /clubs/{clubId}/courts/{courtId}` — Edit court. **Connected**
- `PATCH /clubs/{clubId}/courts/{courtId}/status` — Toggle pitch activity status (Active/Suspended). **Connected**

---

## 4. Bookings & Reviews Endpoints
- `GET /bookings/my-bookings` — Player bookings dashboard log. **Connected**
- `POST /bookings` — Submits a reservation request. **Connected**
- `DELETE /bookings/{bookingId}` — Cancels reservation (subject to 24h constraint). **Connected**
- `GET /reviews/court/{courtId}` — Fetch ratings and player reviews. **Connected**
- `POST /reviews` — Post feedback for completed booking. **Connected**

---

## 5. Tournament & Friendly Match Endpoints
- `GET /friendly-matches` — Explore active open friendly matches. **Connected**
- `GET /friendly-matches/court/{courtId}` — Load friendly matches on a specific court. **Connected**
- `POST /friendly-matches` — Creates friendly match (restricted to Player role). **Connected**
- `GET /tournaments/my` — Lists Owner hosted tournaments. **Connected**
- `POST /tournaments` — Creates a tournament (verifies subscription limits). **Connected**
- `GET /tournaments/{id}/matches` — Get brackets list. **Connected**
