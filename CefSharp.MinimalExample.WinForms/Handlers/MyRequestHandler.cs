namespace CefSharp.MinimalExample.WinForms.Handlers
{
    public class MyRequestHandler : CefSharp.Handler.RequestHandler
    {
        private Settings _settings;

        public MyRequestHandler(Settings settings)
        {
            _settings = settings;
        }

        protected override IResourceRequestHandler GetResourceRequestHandler(
            IWebBrowser chromiumWebBrowser,
            IBrowser browser, 
            IFrame frame, 
            IRequest request,
            bool isNavigation, 
            bool isDownload, 
            string requestInitiator, 
            ref bool disableDefaultHandling)
        {
            return new MyResourceRequestHandler(_settings);
        }
    }
}
