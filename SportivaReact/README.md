# Sportiva React Dashboard

This is the premium Glassmorphic Single Page Application (SPA) dashboard for the Sportiva Platform, built using **React (Vite)**, **Tailwind CSS**, and **SignalR** (real-time communication client).

---

## 🚀 How to Run locally

### 1. Install Node.js
Ensure you have Node.js installed on your machine. If not:
- Download and install the LTS version from [nodejs.org](https://nodejs.org/).
- Verify installation in terminal:
  ```bash
  node -v
  npm -v
  ```

### 2. Install Project Dependencies
Navigate to this folder (`SportivaReact`) in your terminal and run:
```bash
npm install
```

### 3. Run Development Server
Start the frontend development server:
```bash
npm run dev
```
By default, the Vite development server will boot up and be accessible on:
👉 **[http://localhost:3000](http://localhost:3000)**

*Note: The frontend is configured to automatically connect to the C# Backend API running on `http://localhost:5250`.*

---

## 📁 Project Structure

```text
SportivaReact/
├── public/                 # Static assets
├── src/
│   ├── contexts/
│   │   └── SocketContext.jsx # SignalR Chat & Notifications hub connection state provider
│   ├── services/
│   │   └── api.js          # Unified fetch client injecting JWT auth headers
│   ├── App.jsx             # Main dashboard view switcher & auth login guard
│   ├── index.css           # Tailwind CSS directives & global dark glassmorphism variables
│   └── main.jsx            # DOM mounting root
├── index.html              # Entry HTML template
├── package.json            # Node project configuration and dependencies
├── vite.config.js          # Vite bundler parameters
├── tailwind.config.js      # Tailwind CSS selectors configuration
└── postcss.config.js       # PostCSS compiler configuration
```

---

## 🛠️ Build for Production
To bundle the React frontend application into static assets ready for deployment:
```bash
npm run build
```
This command outputs optimized html, CSS, and JS files inside the `dist/` directory. You can host these files on any CDN or copy them into the backend's `wwwroot` directory to host them directly via the ASP.NET Core Kestrel web server.
