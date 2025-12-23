mergeInto(LibraryManager.library, {
  OpenFullScreen: function () {
    if (!document.fullscreenElement) 
    {
        document.documentElement.requestFullscreen()
    } else 
    {
        if (document.exitFullscreen) 
        {
            document.exitFullscreen()
        }
    }
  },
});