/* PWA SW – dev */
const SW_VERSION = 'v3-dev';

self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', e => e.waitUntil(self.clients.claim()));

self.addEventListener('fetch', event => {
  const req = event.request;
  const url = new URL(req.url);

  // 1) Never handle cross-origin (API, Blob SAS, etc.)
  if (url.origin !== self.location.origin) return;

  // 2) Never handle API calls
  if (url.pathname.startsWith('/api/')) return;

  // Let the browser handle everything else in dev (no offline caching here)
});
