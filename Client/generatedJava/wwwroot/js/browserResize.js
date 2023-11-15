window.browserResize = {
    registerResizeCallback: function () {
        window.addEventListener("resize", browserResize.resized);
    },
    resized: function () {
        DotNet.invokeMethodAsync("Client", 'OnBrowserResize').then(function (data) { return data; });
    }
};
//# sourceMappingURL=browserResize.js.map