using AutoMapper;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cooking
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("Log.log"));
            Trace.AutoFlush = true;
            AppDomain.CurrentDomain.UnhandledException += FatalUnhandledException;

            DatabaseService.InitDatabase();
        }

        private const string dateTimeFormat = "dd.MM.yyyy hh:mm";

        private void FatalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                var exceptionDescription = new StringBuilder();

                exceptionDescription.AppendLine(exception.Message);
                exceptionDescription.AppendLine(exception.StackTrace);

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    exceptionDescription.AppendLine(exception.Message);
                    exceptionDescription.AppendLine(exception.StackTrace);
                }
                Trace.TraceError($"[{DateTime.Now.ToString(dateTimeFormat, CultureInfo.InvariantCulture)}] {exceptionDescription.ToString()}");
            }
        }
    }
}
