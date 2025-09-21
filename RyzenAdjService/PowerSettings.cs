using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using RyzenAdjService;

namespace RyzenAdjService
{
  /// <summary>
  /// Windows power scheme identifiers
  /// </summary>
  public enum PowerScheme
  {
    /// <summary>
    /// Better Battery - Energy saving mode
    /// </summary>
    BetterBattery,

    /// <summary>
    /// Better Performance - Balanced mode (default)
    /// </summary>
    BetterPerformance,

    /// <summary>
    /// Best Performance - High performance mode
    /// </summary>
    BestPerformance,

    /// <summary>
    /// Unknown or custom power scheme
    /// </summary>
    Unknown
  }

  /// <summary>
  /// AC Line status enumeration
  /// </summary>
  public enum ACLineStatus
  {
    /// <summary>
    /// Running on battery power
    /// </summary>
    Battery = 0,

    /// <summary>
    /// Connected to AC power
    /// </summary>
    Powered = 1,

    /// <summary>
    /// Unknown power status
    /// </summary>
    Unknown = 255
  }

  /// <summary>
  /// Represents the current power state of the system
  /// </summary>
  /// <param name="AcLineStatus">Current AC line status (battery/powered/unknown)</param>
  /// <param name="Slider">Current power scheme slider setting corresponding to the current power source</param>
  public record PowerState(ACLineStatus AcLineStatus, PowerScheme Slider)
  {
    /// <summary>
    /// Creates a PowerState by reading current system values
    /// </summary>
    /// <returns>Current system power state</returns>
    public static PowerState GetCurrent()
    {
      ACLineStatus acLineStatus = PowerSettings.GetACLineStatus();
      PowerScheme currentSlider = acLineStatus == ACLineStatus.Battery
        ? PowerSettings.GetCurrentDcSlider()
        : PowerSettings.GetCurrentAcSlider();

      return new PowerState(acLineStatus, currentSlider);
    }

    /// <summary>
    /// Checks if the power state has changed compared to another state
    /// </summary>
    /// <param name="other">The other power state to compare against</param>
    /// <returns>True if any value has changed</returns>
    public bool HasChangedFrom(PowerState other)
    {
      return AcLineStatus != other.AcLineStatus ||
             Slider != other.Slider;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct SystemPowerStatus
  {
    public Byte ACLineStatus;
    public Byte BatteryFlag;
    public Byte BatteryLifePercent;
    public Byte Reserved1;
    public Int32 BatteryLifeTime;
    public Int32 BatteryFullLifeTime;
  }

  public static class PowerSettings
  {
    // Power scheme GUID constants
    private const string BetterBatteryGuid = "961cc777-2547-4f9d-8174-7d86181b8a7a";
    private const string BetterPerformanceGuid = "00000000-0000-0000-0000-000000000000";
    private const string BestPerformanceGuid = "ded574b5-45a0-4f42-8737-46345c09c238";

    // Windows API imports
    [DllImport("kernel32.dll")]
    private static extern Boolean GetSystemPowerStatus(out SystemPowerStatus sps);
    // Windows API constants for power notifications
    public const uint DEVICE_NOTIFY_CALLBACK = 0x00000002;
    public const uint PBT_APMSUSPEND = 0x0004;
    public const uint PBT_APMRESUMESUSPEND = 0x0007;

    // Delegate for the callback function
    public delegate uint DeviceNotifyCallbackRoutine(IntPtr Context, uint Type, IntPtr Setting);

    // Structure for device notification
    [StructLayout(LayoutKind.Sequential)]
    public struct DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS
    {
      public DeviceNotifyCallbackRoutine Callback;
      public IntPtr Context;
    }

    // Windows API imports
    [DllImport("powrprof.dll", SetLastError = true)]
    public static extern uint PowerRegisterSuspendResumeNotification(uint Flags, ref DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS Recipient, ref IntPtr RegistrationHandle);

    // DateTime tracking - null means sleeping
    private static long? lastResumedTicks = 0;

    public static bool IsSleeping()
    {
      return lastResumedTicks == null;
    }

    public static bool IsRecentlyResumed(int secondsThreshold)
    {
      long? ticks = lastResumedTicks;
      if (ticks == null)
        return false; // sleeping
      DateTime lastResumed = new DateTime(ticks.Value, DateTimeKind.Local);
      return (DateTime.Now - lastResumed).TotalSeconds < secondsThreshold;
    }

    private static IntPtr registrationHandle = IntPtr.Zero;

    public static void Initialize()
    {
      DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS recipient = new DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS();
      recipient.Callback = new DeviceNotifyCallbackRoutine(DeviceNotifyCallback);
      recipient.Context = IntPtr.Zero;

      uint result = PowerRegisterSuspendResumeNotification(DEVICE_NOTIFY_CALLBACK, ref recipient, ref registrationHandle);

      if (result != 0)
      {
        throw new Exception("Error registering for power notifications: " + Marshal.GetLastWin32Error());
      }
      else
      {
        Logger.WriteLine("Successfully registered for Modern Standby power notifications!");
      }
    }

    private static uint DeviceNotifyCallback(IntPtr Context, uint Type, IntPtr Setting)
    {
      switch (Type)
      {
        case PBT_APMSUSPEND:
          // Entered Modern Standby - set to sleeping state
          lastResumedTicks = null; // Set to sleeping
          Logger.WriteLine("Suspend detected");
          break;
        case PBT_APMRESUMESUSPEND:
          // Exited Modern Standby - set resume time
          lastResumedTicks = DateTime.Now.Ticks; // Set resume time
          Logger.WriteLine("Resume detected");
          break;
      }

      return 0; // ERROR_SUCCESS
    }

    /// <summary>
    /// Converts a power scheme GUID string to PowerScheme enum
    /// </summary>
    /// <param name="guidString">The power scheme GUID string</param>
    /// <returns>The corresponding PowerScheme enum value</returns>
    private static PowerScheme ParsePowerSchemeGuid(string guidString)
    {
      return guidString?.ToLowerInvariant() switch
      {
        BetterBatteryGuid => PowerScheme.BetterBattery,
        BetterPerformanceGuid => PowerScheme.BetterPerformance,
        BestPerformanceGuid => PowerScheme.BestPerformance,
        _ => PowerScheme.Unknown
      };
    }

    // Power status methods
    public static ACLineStatus GetACLineStatus()
    {
      GetSystemPowerStatus(out SystemPowerStatus systemPowerStatus);
      return systemPowerStatus.ACLineStatus switch
      {
        0 => ACLineStatus.Battery,
        1 => ACLineStatus.Powered,
        _ => ACLineStatus.Unknown
      };
    }

    public static PowerScheme GetCurrentAcSlider()
    {
      using var powerkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\Power\\User\\PowerSchemes\\");
      string guidString = powerkey?.GetValue("ActiveOverlayACPowerScheme")?.ToString() ?? string.Empty;
      return ParsePowerSchemeGuid(guidString);
    }

    public static PowerScheme GetCurrentDcSlider()
    {
      using var powerkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\Power\\User\\PowerSchemes\\");
      string guidString = powerkey?.GetValue("ActiveOverlayDCPowerScheme")?.ToString() ?? string.Empty;
      return ParsePowerSchemeGuid(guidString);
    }
  }
}