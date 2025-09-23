// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const SW_VERSION = 'v4-prod';
const CACHE_PREFIX = 'app-cache-';
const CACHE_NAME = `${CACHE_PREFIX}${SW_VERSION}`;

const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    self.skipWaiting();
    event.waitUntil((async () => {
        const cache = await caches.open(CACHE_NAME);
        // Optionally pre-cache your static shell here.
        // await cache.addAll(['/index.html']);
    })());
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    event.waitUntil((async () => {
        // Claim control
        await self.clients.claim();
        // Clean old caches
        const keys = await caches.keys();
        await Promise.all(keys
            .filter(k => k.startsWith(CACHE_PREFIX) && k !== CACHE_NAME)
            .map(k => caches.delete(k)));
    })());
}

function shouldBypass(req) {
    const url = new URL(req.url);
    // Bypass cross-origin (Azure API, Blob SAS)
    if (url.origin !== self.location.origin) return true;
    // Bypass API calls
    if (url.pathname.startsWith('/api/')) return true;
    // Bypass short-lived signed URLs
    if (url.search.includes('sig=')) return true;
    return false;
}

function isStaticAsset(req) {
    const url = new URL(req.url);
    return (
        url.origin === self.location.origin &&
        (url.pathname.startsWith('/_framework/') ||
         url.pathname.startsWith('/_content/') ||
         url.pathname.endsWith('.css') ||
         url.pathname.endsWith('.js') ||
         url.pathname.endsWith('.wasm') ||
         url.pathname.endsWith('.woff') ||
         url.pathname.endsWith('.woff2') ||
         url.pathname.endsWith('.png') ||
         url.pathname.endsWith('.jpg') ||
         url.pathname.endsWith('.svg'))
    );
}

self.addEventListener('fetch', event => {
  const req = event.request;

  // Don’t intercept API, cross-origin, or SAS requests
  if (shouldBypass(req)) return;

  // Static assets: cache-first with safe fallback
  if (req.method === 'GET' && isStaticAsset(req)) {
    event.respondWith((async () => {
      const cache = await caches.open(CACHE_NAME);
      const cached = await cache.match(req, { ignoreSearch: true });
      if (cached) return cached;

      try {
        const resp = await fetch(req);
        if (resp && resp.ok && resp.type !== 'opaque') {
          cache.put(req, resp.clone());
        }
        return resp;
      } catch {
        // Always return a Response, never undefined/null
        return Response.error();
      }
    })());
    return;
  }

  // All other same-origin requests: network-first, fallback to cache if available
  event.respondWith((async () => {
    try {
      return await fetch(req);
    } catch {
      const cache = await caches.open(CACHE_NAME);
      const cached = await cache.match(req, { ignoreSearch: true });
      return cached || Response.error();
    }
  })());
});
