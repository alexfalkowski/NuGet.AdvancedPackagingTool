﻿namespace NuGet.AdvancedPackagingTool.Test.Integration
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using NuGet.AdvancedPackagingTool.Core;

    public class PackagesWebServer
    {
        private Process process;

        public void Startup()
        {
            if (Debugger.IsAttached)
            {
                return;
            }

            foreach (var runningProcess in Process.GetProcessesByName("iisexpress"))
            {
                KillProcess(runningProcess);
            }

            var currentExecutingDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            if (currentExecutingDirectoryName == null)
            {
                return;
            }

            const string FileName = @"C:\Program Files (x86)\IIS Express\iisexpress.exe";
            var currentPath = new Uri(currentExecutingDirectoryName).LocalPath;
            var path = Path.GetFullPath(Path.Combine(currentPath, @"..\..\..\NuGet.AdvancedPackagingTool.Service"));
            var arguments = string.Format(CultureInfo.CurrentCulture, @"/path:""{0}"" /port:1544", path);

            IProcess backgroundProcess = new BackgroundProcess();
            this.process = backgroundProcess.CreateStartedProcess(FileName, arguments);
            Thread.Sleep(3000);
        }

        public void Stop()
        {
            if (Debugger.IsAttached)
            {
                return;
            }

            KillProcess(this.process);
        }

        private static void KillProcess(Process processToKill)
        {
            if (processToKill == null)
            {
                return;
            }

            try
            {
                processToKill.Kill();
            }
            catch (InvalidOperationException)
            {
            }
            catch (Win32Exception)
            {
            }
        }
    }
}
