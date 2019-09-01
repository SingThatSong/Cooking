﻿using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Data.Context
{
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider) { }

        private ConsoleLogger logger;

        public ILogger CreateLogger(string categoryName)
        {
            Debug.WriteLine($"Event for {categoryName}");
            return logger ?? (logger = new ConsoleLogger());
        }

        public void Dispose() { }
    }
}
