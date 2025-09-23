/* SW (dev) */
const SW_VERSION = 'v4-dev';

self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', e => e.waitUntil(self.clients.claim()));

self.addEventListener('fetch', event => {
  const req = event.request;
  const url = new URL(req.url);

  // 1) Never handle cross-origin (e.g., your Azure API, Blob SAS)
  if (url.origin !== self.location.origin) return;

  // 2) Never handle API calls
  if (url.pathname.startsWith('/api/')) return;

  // 3) In dev, let the browser fetch everything
  // If you previously had event.respondWith(caches.match(req)) without a fallback,
  // remove it — that is what causes "Failed to convert value to 'Response'".
});
