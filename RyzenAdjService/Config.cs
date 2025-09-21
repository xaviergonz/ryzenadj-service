using System;
using System.Linq;
using RyzenAdjService;

namespace RyzenAdjService
{
  public static class Config
  {
    public static int RepeatWaitTimeSeconds { get; } = 10;

    // if monitorField is set, this script does only adjust values if something did revert your monitored value. Clear monitorField String to disable monitoring
    // This needs to be an value which actually gets overwritten by your device firmware/software if no changes get detected, your settings will not reapplied
    public static FieldMonitorState[] FieldMonitorStates { get; } =
    [
        new("fast_limit", 0.1),
            new("stapm_limit", 0.1),
            new("slow_limit", 0.1),
        ];

    public static void AdjustRyzen(PowerState powerState)
    {
      AdjustField.Adjust("stapm_limit", 28000, FieldMonitorStates);
      AdjustField.Adjust("fast_limit", 28000, FieldMonitorStates);
      AdjustField.Adjust("slow_limit", 28000, FieldMonitorStates);
    }
  }
}