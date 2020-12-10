﻿// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp.MinimalExample.WinForms.Handlers;
using CefSharp.WinForms;
using System;
using System.IO;
using System.Windows.Forms;

namespace CefSharp.MinimalExample.WinForms
{
    public class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();

#if NETCOREAPP
            //We are using our current exe as the BrowserSubProcess
            //Multiple instances will be spawned to handle all the 
            //Chromium proceses, render, gpu, network, plugin, etc.
            var subProcessExe = new CefSharp.BrowserSubprocess.BrowserSubprocessExecutable();
            var result = subProcessExe.Main(args);
            if (result > 0)
            {
                return result;
            }
#endif

            var cacheDirBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = cacheDirBase
            };

#if NETCOREAPP
            //We use our Applications exe as the BrowserSubProcess, multiple copies
            //will be spawned
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            settings.BrowserSubprocessPath = exePath;
#endif

            //Example of setting a command line argument
            //Enables WebRTC
            // - CEF Doesn't currently support permissions on a per browser basis see https://bitbucket.org/chromiumembedded/cef/issues/2582/allow-run-time-handling-of-media-access
            // - CEF Doesn't currently support displaying a UI for media access permissions
            //
            //NOTE: WebRTC Device Id's aren't persisted as they are in Chrome see https://bitbucket.org/chromiumembedded/cef/issues/2064/persist-webrtc-deviceids-across-restart
            settings.CefCommandLineArgs.Add("enable-media-stream");
            //https://peter.sh/experiments/chromium-command-line-switches/#use-fake-ui-for-media-stream
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
            //For screen sharing add (see https://bitbucket.org/chromiumembedded/cef/issues/2582/allow-run-time-handling-of-media-access#comment-58677180)
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
            settings.CefCommandLineArgs.Add("disable-application-cache", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            var browser = new BrowserForm();

            Application.Run(browser);

            return 0;
        }
    }
}
