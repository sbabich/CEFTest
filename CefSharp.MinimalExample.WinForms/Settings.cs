using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefSharp.MinimalExample.WinForms
{
    /// <summary>
    /// Настойка подмен и блокировок
    /// </summary>
    public class Settings
    {
        public List<string> BlockedUrls { get; } = new List<string>();

        public ReplacementPair PostDataChange { get; } = new ReplacementPair();

        public ReplacementPair ReplaceBody { get; } = new ReplacementPair();
    }

    public class ReplacementPair
    { 
        public string WhatDoINeetToFind { get; set; }
        public string WhatDoINeetToPaste { get; set; }
    }
}
