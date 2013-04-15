using System;
using System.Linq;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using AzureTraceListeners.Listeners;

namespace PhluffyFotos.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            SetUpLogginForRole();
            
            return base.OnStart();
        }
  
        private void SetUpLogginForRole()
        {
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Error;

            string connectionString = RoleEnvironment.GetConfigurationSettingValue("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString");

            Trace.Listeners.Add(new AzureTableTraceListener("WebRole", connectionString, "TraceLogs"));
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

        }
    }
}
