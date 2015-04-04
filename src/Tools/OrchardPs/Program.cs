﻿namespace OrchardPs 
{
    using System;
    using Autofac.Core.Registration;
    using OrchardPs.Console;
    using OrchardPs.Host;

    public static class Program
    {
        private static bool running = true;
        private static OrchardHostContextProvider hostContextProvider;
        private static OrchardHostContext hostContext;

        public static int Main(string[] args)
        {
            System.Console.WriteLine(Banner.GetBanner());

            hostContextProvider = new OrchardHostContextProvider();

            try
            {
                hostContext = InitializeOrchardHost();
            }
            catch (Exception ex)
            {
                // NOTE: Currently Autofac.Core.Registration.ComponentNotRegisteredException is not serializable,
                // so we need to workaround this issue by reading the exception's message.
                if ((ex is ComponentNotRegisteredException) || ex.Message.Contains("ComponentNotRegisteredException"))
                {
                    ConsoleHelper.WriteToConsole(
                       "Failed to initialize PowerShell engine host. " +
                       "Please make sure that the Proligence.PowerShell.Provider Orchard module is enabled." +
                       Environment.NewLine,
                       ConsoleColor.Red);

                    return -1;   
                }

                throw;
            }

            System.Console.CancelKeyPress += OnCancelKeyPress;

            var session = hostContext.Session;
            var connection = hostContext.OrchardHost.Connection;
            while (running)
            {
                string input = connection.GetInput();

                if ((input == "clear") || (input == "cls"))
                {
                    System.Console.Clear();
                }
                else if (input == "exit")
                {
                    running = false;
                    break;
                }
                else
                {
                    session.ProcessInput(input);                    
                }
            }

            hostContextProvider.Shutdown(hostContext);
            
            return 0;
        }

        private static OrchardHostContext InitializeOrchardHost()
        {
            OrchardHostContext context = hostContextProvider.CreateContext();
            if (context.Session == null)
            {
                context = hostContextProvider.CreateContext();
            }
            else if (context.Session == null)
            {
                hostContextProvider.Shutdown(context);
                throw new ApplicationException("Failed to initialize Orchard session.");
            }
            
            return context;
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            running = false;
            hostContextProvider.Shutdown(hostContext);
        }
    }
}