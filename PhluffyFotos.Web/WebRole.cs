using System;
using System.Linq;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Collections.Generic;
using AzureTraceListeners.Listeners;

namespace MvcWebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Error;

            Trace.Listeners.Add(new AzureTableTraceListener("app", "useDevelopmentStorage=true", "TraceLogs"));
            //Windows Event Logs
            dmc.WindowsEventLog.DataSources.Add("System!*");
            dmc.WindowsEventLog.DataSources.Add("Application!*");
            dmc.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromSeconds(1.0);
            dmc.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Warning;

            //Azure Trace Logs
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Warning;

            //Crash Dumps
            CrashDumps.EnableCollection(true);

            //IIS Logs
            dmc.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);

            
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", dmc);
            
            Trace.Listeners.Add(new AzureTableTraceListener("app", "useDevelopmentStorage=true", "TraceLogs"));
            
            Trace.Write("*********************************************Message*********************************************");
            Trace.TraceError("*********************************************Message*********************************************");
            Trace.TraceWarning("*********************************************Message*********************************************");
            Trace.TraceInformation("*********************************************Message*********************************************");
            Trace.Flush();// ("*********************************************Message*********************************************");
            Trace.TraceError("*********************************************Message*********************************************");
            

            return base.OnStart();


        }
    }
}
