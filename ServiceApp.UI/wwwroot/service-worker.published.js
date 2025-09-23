// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const SW_VERSION = 'v3-prod';
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
        // Cache only static, same-origin assets. Do NOT pre-cache SAS or API.
        // Optionally add your static shell files here if you don’t use Blazor’s generated manifest.
        // await cache.addAll(['/index.html', '/css/app.css', '/js/app.js']);
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
    // Bypass cross-origin (e.g., https://*.azurewebsites.net, https://*.blob.core.windows.net)
    if (url.origin !== self.location.origin) return true;
    // Bypass API calls
    if (url.pathname.startsWith('/api/')) return true;
    // Bypass SAS/short-lived URLs
    if (url.search.includes('sig=')) return true;
    return false;
}

function isStaticAsset(req) {
    const url = new URL(req.url);
    // Only cache same-origin static assets
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

async function onFetch(event) {
    const req = event.request;

    if (shouldBypass(req)) {
        // Let the network handle API, cross-origin, SAS, etc.
        return;
    }

    let cachedResponse = null;
    if (req.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache,
        // unless that request is for an offline resource.
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = req.mode === 'navigate'
            && !manifestUrlList.some(url => url === req.url);

        const request = shouldServeIndexHtml ? 'index.html' : req;
        const cache = await caches.open(CACHE_NAME);
        cachedResponse = await cache.match(request);
    }

    if (!isStaticAsset(req) || req.method !== 'GET') {
        // Navigation/doc requests: network first, fallback to cache
        event.respondWith((async () => {
            try {
                return await fetch(req);
            } catch {
                const cache = await caches.open(CACHE_NAME);
                const cached = await cache.match(req, { ignoreSearch: true });
                return cached || Response.error();
            }
        })());
        return;
    }

    // Static assets: cache-first, then network update
    event.respondWith((async () => {
        const cache = await caches.open(CACHE_NAME);
        const cached = await cache.match(req, { ignoreSearch: true });
        if (cached) return cached;

        const resp = await fetch(req);
        // Avoid caching opaque or error responses
        if (resp && resp.ok && resp.type !== 'opaque') {
            cache.put(req, resp.clone());
        }
        return resp;
    })());
}
