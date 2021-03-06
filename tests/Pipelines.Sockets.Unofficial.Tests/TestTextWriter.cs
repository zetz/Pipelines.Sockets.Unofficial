﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Xunit.Abstractions;

namespace Pipelines.Sockets.Unofficial.Tests
{
    internal sealed class TestTextWriter : TextWriter
    {
        public static TestTextWriter Create(ITestOutputHelper log) => log == null ? null : new TestTextWriter(log);
        private readonly ITestOutputHelper _log;
        private readonly TextWriter _text;

        public override Encoding Encoding => Encoding.Unicode;

        public TestTextWriter(ITestOutputHelper log)
        {
            _log = log;
#if DEBUG
            SocketConnection.SetLog(this);
#endif
        }
        public TestTextWriter(TextWriter text)
        {
            _text = text;
#if DEBUG
            SocketConnection.SetLog(text);
#endif
        }
        public override void WriteLine(string value)
        {
            var tmp = _log;
            if(tmp != null)
            {
                lock(tmp)
                {
                    tmp.WriteLine(value);
                }
            }
            var tmp2 = _text;
            if (tmp2 != null)
            {
                lock(tmp2)
                {
                    tmp2.WriteLine(value);
                }
            }            
        }

        [Conditional("DEBUG")]
        public void DebugLog(string message = "", [CallerMemberName] string caller = null)
        {
            var thread = Thread.CurrentThread;
            var name = thread.Name;
            if (string.IsNullOrWhiteSpace(name)) name = thread.ManagedThreadId.ToString();
            
            WriteLine($"[{name}:{caller}] {message}");
        }

        [Conditional("VERBOSE")]
        public void DebugLogVerbose(string message = "", [CallerMemberName] string caller = null) => DebugLog(message, caller);

        [Conditional("DEBUG")]
        public void DebugLogWriteLine() => WriteLine("");

        [Conditional("VERBOSE")]
        public void DebugLogVerboseWriteLine() => DebugLogWriteLine();
    }
}
