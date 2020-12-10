using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CefSharp.MinimalExample.WinForms.Handlers
{
    public class MyFindAndReplaceFilter : IResponseFilter
    {
        private Dictionary<string, string> _dictionary;
        private List<byte> _dataOutBuffer = new List<byte>();

        public MyFindAndReplaceFilter(Dictionary<string, string> dictionary)
        {
            this._dictionary = dictionary;
        }

        bool IResponseFilter.InitFilter()
        {
            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                var maxWrite = Math.Min(_dataOutBuffer.Count, dataOut.Length);

                if (maxWrite > 0)
                {
                    dataOut.Write(_dataOutBuffer.ToArray(), 0, (int)maxWrite);
                    dataOutWritten += maxWrite;
                }

                if (maxWrite < _dataOutBuffer.Count)
                {     
                    _dataOutBuffer.RemoveRange(0, (int)(maxWrite - 1));
                    return FilterStatus.NeedMoreData;
                }

                _dataOutBuffer.Clear();

                return FilterStatus.Done;
            }

            dataInRead = dataIn.Length;

            var dataInBuffer = new byte[(int)dataIn.Length];
            dataIn.Read(dataInBuffer, 0, dataInBuffer.Length);

            _dataOutBuffer.AddRange(dataInBuffer);

            dataOutWritten = 0;

            if (dataIn.Length < dataOut.Length)
            {
                // Поиск чего б поменять
                var dataString = Encoding.UTF8.GetString(_dataOutBuffer.ToArray());
                foreach (var d in _dictionary)
                {
                    var pattern =
                        new Regex(d.Key,
                            RegexOptions.Compiled |
                            RegexOptions.Singleline);

                    var match = pattern.Match(dataString);
                    while (match.Success)
                    {
                        dataString = dataString.Replace(match.Value, d.Value);
                        match = pattern.Match(dataString);
                    }
                }

                var bytes = Encoding.UTF8.GetBytes(dataString);

                _dataOutBuffer.Clear();
                _dataOutBuffer.AddRange(bytes);

                var maxWrite = Math.Min(_dataOutBuffer.Count, dataOut.Length);

                if (maxWrite > 0)
                {
                    dataOut.Write(_dataOutBuffer.ToArray(), 0, (int)maxWrite);
                    dataOutWritten += maxWrite;
                }

                if (maxWrite < _dataOutBuffer.Count)
                {
                    _dataOutBuffer.RemoveRange(0, (int)(maxWrite - 1));

                    return FilterStatus.NeedMoreData;
                }

                _dataOutBuffer.Clear();

                return FilterStatus.Done;
            }
            
            return FilterStatus.NeedMoreData;
        }

        public void Dispose() { }
    }
}
