﻿using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Cooking.Data.Context
{
    public class ConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => new Scope();
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Debug.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff", null)}] {formatter(state, exception)}");
        }
    }
}
