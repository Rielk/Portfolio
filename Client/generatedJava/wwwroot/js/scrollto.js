function scrollToId(id) {
    var element = document.getElementById(id);
    if (element instanceof HTMLElement) {
        element.scrollIntoView({
            behavior: "smooth",
            block: "start",
            inline: "nearest"
        });
    }
}
;
//# sourceMappingURL=scrollto.js.map