(function () {
    if (!('serviceWorker' in navigator)) return;

    let refreshing = false;

    navigator.serviceWorker.addEventListener('controllerchange', function () {
        if (refreshing) return;
        refreshing = true;
        window.location.reload();
    });

    async function registerAndCheck() {
        try {
            const registration = await navigator.serviceWorker.register('/service-worker.js', {
                updateViaCache: 'none'
            });

            const checkForUpdate = function () {
                registration.update().catch(function () { });
            };

            checkForUpdate();

            document.addEventListener('visibilitychange', function () {
                if (document.visibilityState === 'visible') checkForUpdate();
            });
            window.addEventListener('focus', checkForUpdate);
            window.addEventListener('online', checkForUpdate);
            window.addEventListener('pageshow', checkForUpdate);
            window.setInterval(checkForUpdate, 5 * 60 * 1000);
        } catch (error) {
            console.warn('Kunne ikke kontrollere appoppdatering.', error);
        }
    }

    if (document.readyState === 'loading') {
        window.addEventListener('DOMContentLoaded', registerAndCheck, { once: true });
    } else {
        registerAndCheck();
    }
})();
