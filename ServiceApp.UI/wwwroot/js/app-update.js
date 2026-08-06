(function () {
    if (!('serviceWorker' in navigator)) return;

    let refreshing = false;

    navigator.serviceWorker.addEventListener('controllerchange', function () {
        if (refreshing) return;
        refreshing = true;
        window.location.reload();
    });

    window.addEventListener('load', async function () {
        try {
            const registration = await navigator.serviceWorker.register('/service-worker.js', {
                updateViaCache: 'none'
            });

            await registration.update();

            document.addEventListener('visibilitychange', function () {
                if (document.visibilityState === 'visible') {
                    registration.update().catch(function () { });
                }
            });

            window.setInterval(function () {
                registration.update().catch(function () { });
            }, 60 * 60 * 1000);
        } catch (error) {
            console.warn('Kunne ikke kontrollere appoppdatering.', error);
        }
    });
})();
