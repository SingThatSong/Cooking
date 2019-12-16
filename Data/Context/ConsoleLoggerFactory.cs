using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Data.Context
{
    public sealed class ConsoleLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider) { }

        private ConsoleLogger logger;

        public ILogger CreateLogger(string categoryName)
        {
            Debug.WriteLine($"{categoryName}");
            return logger ?? (logger = new ConsoleLogger());
        }

        public void Dispose() { }
    }
}
