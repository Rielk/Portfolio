window.browserResize = {
    registerResizeCallback: function () {
        window.addEventListener("resize", browserResize.resized);
    },
    resized: function () {
        DotNet.invokeMethodAsync("Client", 'OnBrowserResize').then(data => data);
    }
}