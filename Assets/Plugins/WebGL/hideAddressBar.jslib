mergeInto(LibraryManager.library, {
    HideAddressBar: function () {
        setTimeout(function () {
            window.scrollTo(0, 1);
        }, 100);
    }
});