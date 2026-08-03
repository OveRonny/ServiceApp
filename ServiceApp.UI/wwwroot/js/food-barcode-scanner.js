window.foodBarcodeScanner = (() => {
    let controls = null;
    let dotNetReference = null;

    async function stop() {
        if (controls) {
            controls.stop();
            controls = null;
        }
        dotNetReference = null;
    }

    async function start(videoElementId, reference) {
        await stop();
        if (!window.ZXingBrowser)
            throw new Error("Strekkodeskanneren kunne ikke lastes.");
        if (!navigator.mediaDevices?.getUserMedia)
            throw new Error("Kamera krever HTTPS og en nettleser med kamerastøtte.");

        dotNetReference = reference;
        const video = document.getElementById(videoElementId);
        const reader = new ZXingBrowser.BrowserMultiFormatReader();
        controls = await reader.decodeFromConstraints(
            { video: { facingMode: { ideal: "environment" } }, audio: false },
            video,
            async (result) => {
                if (!result || !dotNetReference) return;
                const value = result.getText();
                const callback = dotNetReference;
                await stop();
                await callback.invokeMethodAsync("BarcodeDetected", value);
            });
    }

    return { start, stop };
})();
