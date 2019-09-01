using System;
using Microsoft.Extensions.Logging;

namespace Data.Context
{
    public class ConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => new Scope();
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {formatter(state, exception)}");
        }
    }
}
