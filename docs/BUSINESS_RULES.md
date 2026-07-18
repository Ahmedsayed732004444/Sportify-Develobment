# Sportify Business Rules Manual

This document details all implemented and enforced business logic constraints and flows across the Sportify ecosystem.

---

## 1. Role-Based Permissions & Guardrails
- **Guest (Unauthenticated User):**
  - Can search sports fields, view complexes, inspect public matches, and view pricing tiers.
  - Restricted from booking courts, hosting matches, registering for tournaments, chatting, commenting, or settings updates.
- **Player (Athlete):**
  - Full marketplace access: can book courts, pay via cash or card, and rate complexes.
  - Can host or participate in **Friendly Matches**.
  - Can register as a participant/team in **Tournaments**.
  - Restricted from: Adding clubs/courts, hosting tournaments, accessing owner views, or updating club configurations.
- **Owner (Sports Club Partner):**
  - Can register clubs, add courts, modify schedules, view bookings, and run reviews.
  - Can host and organize **Tournaments** (brackets, matches, score entries).
  - Restricted from: Creating or participating in **Friendly Matches** (they only manage official club events). Zero access to Player messaging services.
- **Admin (System Moderator):**
  - Reviews onboarding applications for new venues, approves manual subscription plan requests, monitors general listings.
  - Restricted from: Creating friendly matches or listing personal courts directly without an owner link.

---

## 2. Onboarding & Club Approval Flow
1. **Application Submission:** Authenticated players click "Become Partner" -> View pricing plans -> Select plan -> Complete wizard (Representative name, 14-digit National ID, phone, proposed club details, maps link, sports pitches list, and descriptions).
2. **Review State:** Club is initialized in the database as `IsActive = false` (Inactive). The user profile state becomes `Pending`.
3. **Admin Review:** Administrator reviews credentials in the Onboarding Dashboard.
4. **Activation:** Admin clicks "Approve & Activate". The user role escalates to `Owner`, the club status flips to `IsActive = true`, and a default subscription plan is registered.

---

## 3. Subscription Management & Tier Constraints
- **Manual Request Workflow:**
  - Owners cannot modify their active billing subscription directly.
  - To upgrade or renew, the owner clicks "Request Renewal" or "Request Upgrade" inside the billing tab.
  - They submit a request detailing notes, representative phone, and the preferred plan tier.
  - An Admin manually reviews and clicks "Approve" (which soft-deletes the old active subscription and inserts the new active plan) or "Reject".
- **Subscription Tier Limits:**
  - **Basic Plan (100 EGP/year):** Capped at a maximum of **1 registered club** and **1 active tournament** at any time. Max courts limit is **1**.
  - **Premium Plan (250 EGP/year):** Capped at a maximum of **2 registered clubs** and **3 active tournaments** at any time. Max courts limit is **3**.
  - **Elite Plan (500 EGP/year):** Capped at a maximum of **5 registered clubs** and **10 active tournaments** at any time. Max courts limit is **10**.

---

## 4. Court Bookings & Scheduling
- **Slot Lengths:** Slots are generated as fixed 60-minute blocks (hourly).
- **Price Calculation:** Derived from base rate hourly fees configured in Court Management or overrides configured in Weekly Schedules.
- **Cancellation Lock:** Players are permitted to cancel scheduled bookings only if the start time is at least **24 hours away**. Cancellations within 24 hours are locked.
- **Overlapping Prevention:** Double bookings are mathematically blocked by checking the date, court ID, and start time index against active bookings during the request sequence.

---

## 5. Friendly Matches & Tournament Orchestration
- **Friendly Lobbies:** Players organize public matches, set date/time/sport, and coordinate rosters. Join requests must be accepted by the host. Slot status automatically flips to `Full` once the required players count is satisfied.
- **Tournaments (Leagues):** Owners host tournaments. System automatically checks plan limits (e.g. Basic owners cannot host more than 1 league simultaneously). Bracket matches are populated automatically based on participants.
