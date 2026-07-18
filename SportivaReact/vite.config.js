import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    cors: true,
    proxy: {
      '/auth': 'http://localhost:5250',
      '/clubs': 'http://localhost:5250',
      '/courts': 'http://localhost:5250',
      '/bookings': 'http://localhost:5250',
      '/friendly-matches': 'http://localhost:5250',
      '/membership-requests': 'http://localhost:5250',
      '/me': 'http://localhost:5250',
      '/messages': 'http://localhost:5250',
      '/notifications': 'http://localhost:5250',
      '/posts': 'http://localhost:5250',
      '/profiles': 'http://localhost:5250',
      '/hubs': {
        target: 'http://localhost:5250',
        ws: true
      }
    }
  }
});
