using System;
using System.Threading.Tasks;
using Serilog;

namespace WindowsSetupAssistant.Logic
{
    /// <summary>
    /// Sets up unhandled exception listeners and logging for such
    ///
    /// You must call SetupExceptionHandlingEvents() to set up listeners
    /// </summary>
    public class ExceptionHandler 
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Sets up unhandled exception listeners and logging for such
        ///
        /// You must call SetupExceptionHandlingEvents() to set up listeners
        /// </summary>
        public ExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Sets up listeners for AppDomain.CurrentDomain.UnhandledException and
        /// Application.Current.DispatcherUnhandledException
        ///
        /// Also due to the fact that puppeteer throws AggregateExceptions that can't be caught
        /// (Or at least I can't figure out how to catch them.)
        /// This will check the message on exception.InnerException?.Message to see if it is that exception
        /// and debug log it but do nothing else if it is that specific exception message.
        /// </summary>
        public bool SetupExceptionHandlingEvents()
        {
            _logger.Debug("Setting up exception handling");
            
            AppDomain.CurrentDomain.UnhandledException += async (e, x) => await CurrentDomainUnhandledException(e, x);
            
            // ReSharper disable once UnusedParameter.Local because we're not in control of this method signature
            async Task CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                var exception = e.ExceptionObject as Exception;
                await LogUnhandledException(exception ?? new Exception(), "Application.Current.DispatcherUnhandledException");
                    
            }
            
            _logger.Debug("Exception handling was set up successfully");
            return true;
        }

        private async Task LogUnhandledException(Exception exception, string source)
        {
            var message = $"Unhandled exception ({source})";
            
            try
            {
                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                
                message = $"Unhandled exception in {assemblyName.Name} v{assemblyName.Version}";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                await Task.Delay(1000);
                
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                _logger.Error(exception, message);
                Log.CloseAndFlush();
                
                await Task.Delay(1000);

                Environment.Exit(0);
            }
        }
    }
}