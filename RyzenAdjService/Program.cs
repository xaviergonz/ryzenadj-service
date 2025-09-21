using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using RyzenAdjService;

// Main execution starts here
Logger.WriteLine("RyzenAdj C# Script Starting...");

// Initialize RyzenAdj
var PathToRyzenAdjDlls = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory();
LibRyzenAdj.InitializeRyzenAdj(PathToRyzenAdjDlls);

PowerSettings.Initialize();

Logger.WriteLine("Applying initial power adjustments");
PowerState currentPowerState = PowerState.GetCurrent();
Config.AdjustRyzen(currentPowerState);

LibRyzenAdj.RefreshTable();
foreach (var monitorState in Config.FieldMonitorStates)
{
    monitorState.Target = LibRyzenAdj.GetMonitorValue(monitorState.FieldName);
}

while (true)
{
    if (PowerSettings.IsSleeping() || PowerSettings.IsRecentlyResumed(10))
    {
        string sleepState = PowerSettings.IsSleeping() ? "sleeping" : "recently resumed";
        Logger.WriteLine($"Skipping check, system {sleepState}");
        Thread.Sleep(Config.RepeatWaitTimeSeconds * 1000);
        continue;
    }

    bool doAdjust = false;

    // Check for power state changes
    PowerState newPowerState = PowerState.GetCurrent();
    if (newPowerState.HasChangedFrom(currentPowerState))
    {
        Logger.WriteLine($"Power settings changed from [{currentPowerState}] to [{newPowerState}]");
        currentPowerState = newPowerState;
        doAdjust = true;
    }

    LibRyzenAdj.RefreshTable();
    foreach (var monitorState in Config.FieldMonitorStates)
    {
        double monitorValue = LibRyzenAdj.GetMonitorValue(monitorState.FieldName);
        if (Math.Abs(monitorState.Target - monitorValue) >= monitorState.Epsilon)
        {
            Logger.WriteLine($"{monitorState.FieldName} value unexpectedly changed from {monitorState.Target} to {monitorValue}");
            doAdjust = true;
        }
    }

    if (doAdjust)
    {
        Logger.WriteLine("Re-applying power adjustments");

        Config.AdjustRyzen(currentPowerState);
    }

    Thread.Sleep(Config.RepeatWaitTimeSeconds * 1000);
}
