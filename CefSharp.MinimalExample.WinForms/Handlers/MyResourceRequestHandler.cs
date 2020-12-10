using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace CefSharp.MinimalExample.WinForms.Handlers
{
    public class MyResourceRequestHandler : Handler.ResourceRequestHandler    
    {
        private Settings _settings;
        public MyResourceRequestHandler(Settings settings)
        {
            _settings = settings;
        }

        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            // Замена
            if ("text/html".Equals(response.MimeType) && !string.IsNullOrEmpty(_settings.ReplaceBody.WhatDoINeetToFind))
            {
                var dictionary = new Dictionary<string, string>();
                dictionary.Add(_settings.ReplaceBody.WhatDoINeetToFind, _settings.ReplaceBody.WhatDoINeetToPaste);

                return new MyFindAndReplaceFilter(dictionary);
            }

            return base.GetResourceResponseFilter(chromiumWebBrowser, browser, frame, request, response);
        }
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            // Блокировка
            var url = request?.Url;

            if (!string.IsNullOrEmpty(url))
            {
                lock (_settings.BlockedUrls)
                {
                    if (_settings.BlockedUrls.Count > 0)
                    {
                        if (_settings.BlockedUrls.Any(o =>
                            {
                                try
                                {
                                    var pattern =
                                        new Regex(o,
                                            RegexOptions.Compiled |
                                            RegexOptions.Singleline);

                                    return pattern.IsMatch(url);
                                }
                                catch (ArgumentException)
                                {
                                    return false;
                                }
                            }))
                        {
                            Console.WriteLine($"[BLOCKED] {url}");
                            return CefReturnValue.Cancel;
                        }
                    }
                }
                Console.WriteLine($"[ALLOWED] {url}");
            }

            // Подмена в POST
            if ("POST".Equals(request?.Method)
                && !string.IsNullOrEmpty(_settings.PostDataChange.WhatDoINeetToFind)
                && request.PostData is CefSharp.PostData post
                && post.Elements.Count > 0)
            {
                var pattern =
                    new Regex(_settings.PostDataChange.WhatDoINeetToFind,
                        RegexOptions.Compiled |
                        RegexOptions.Singleline);

                var postData = new PostData();
                foreach (var element in post.Elements)
                {
                    var postData1 = System.Text.Encoding.UTF8.GetString(element.Bytes);

                    var changed = false;
                    var match = pattern.Match(postData1);
                    while (match.Success)
                    {
                        postData1 = postData1.Replace(match.Value, _settings.PostDataChange.WhatDoINeetToPaste);
                        match = pattern.Match(postData1);
                        changed = true;
                    }

                    if (changed)
                        postData.AddData(System.Text.Encoding.UTF8.GetBytes(postData1));
                    else
                        postData.AddData(element.Bytes);
                }

                request.PostData = postData;
            }

            return CefReturnValue.Continue;
        }
    }
}
