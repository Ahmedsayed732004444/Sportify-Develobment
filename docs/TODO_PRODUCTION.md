# Sportify Production Launch Checklist

Follow this master task queue before building and deploying Sportify to staging or production.

---

## 1. CRITICAL PRIORITY (Pre-Deployment Configs)
- [ ] **Configure JWT Credentials:** Update security keys in backend `appsettings.json` with a cryptographically secure 256-bit signature.
- [ ] **Database Connection Strings:** Switch localdb connection strings to production Azure SQL / AWS RDS database endpoints.
- [ ] **SMTP Mail Server Configurations:** Replace mock SMTP server configuration with credentials for a real provider (e.g. SendGrid, Mailgun) for password resets and verification mailings.
- [ ] **Production Build Compilation:** Build the production React static bundle using `npm run build` and resolve any bundle-size chunk optimizations.

---

## 2. HIGH PRIORITY (Payment & Storage Integrations)
- [ ] **Payment Sandbox Integrations:** Set up live Fawry/Paymob gateway test accounts and verify that successful payments activate bookings automatically.
- [ ] **Cloud Storage for Assets:** Configure image uploading to use Cloudinary or AWS S3 instead of local storage (`wwwroot/uploads`) to prevent loss of files on server restarts.
- [ ] **SSL Certificates:** Enforce HTTPS redirects on the hosting server (e.g., Nginx, IIS, Cloudflare).

---

## 3. MEDIUM PRIORITY (Optimizations & Fallbacks)
- [ ] **SignalR SQL Backplane:** Configure a Redis backplane for SignalR hubs to enable scaling across multiple backend servers.
- [ ] **Chat Logging Persistency:** Save SignalR chat history to SQL database table instead of keeping conversations completely memory-bound.
- [ ] **Add Custom 404 View:** Map React Router fallbacks to show a customized, thematic "Page Not Found" screen instead of auto-redirecting.
