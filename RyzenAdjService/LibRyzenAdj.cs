using System;
using System.IO;
using System.Runtime.InteropServices;
using RyzenAdjService;

namespace RyzenAdjService
{
  public static class LibRyzenAdj
  {
    private static IntPtr ry = IntPtr.Zero;

    #region DLL Imports

    [DllImport("libryzenadj.dll")] private static extern IntPtr init_ryzenadj();
    [DllImport("libryzenadj.dll")] private static extern int set_stapm_limit(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_fast_limit(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_slow_limit(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_slow_time(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_stapm_time(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_tctl_temp(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_vrm_current(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_vrmsoc_current(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_vrmmax_current(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_vrmsocmax_current(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_psi0_current(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_psi0soc_current(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_max_gfxclk_freq(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_min_gfxclk_freq(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_max_socclk_freq(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_min_socclk_freq(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_max_fclk_freq(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_min_fclk_freq(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_max_vcn(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_min_vcn(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_max_lclk(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_min_lclk(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_prochot_deassertion_ramp(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_apu_skin_temp_limit(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_dgpu_skin_temp_limit(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_apu_slow_limit(IntPtr ry, [In] uint value);
    [DllImport("libryzenadj.dll")] private static extern int set_power_saving(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern int set_max_performance(IntPtr ry);

    [DllImport("libryzenadj.dll")] public static extern int refresh_table(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern IntPtr get_table_values(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_stapm_limit(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_stapm_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_stapm_time(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_fast_limit(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_fast_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_slow_limit(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_slow_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_apu_slow_limit(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_apu_slow_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrm_current(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrm_current_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrmsoc_current(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrmsoc_current_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrmmax_current(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrmmax_current_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrmsocmax_current(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_vrmsocmax_current_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_tctl_temp(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_tctl_temp_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_apu_skin_temp_limit(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_apu_skin_temp_value(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_dgpu_skin_temp_limit(IntPtr ry);
    [DllImport("libryzenadj.dll")] private static extern float get_dgpu_skin_temp_value(IntPtr ry);

    #endregion

    /// <summary>
    /// Gets the current RyzenAdj handle
    /// </summary>
    public static IntPtr Handle => ry;

    /// <summary>
    /// Initialize RyzenAdj library
    /// </summary>
    /// <param name="pathToRyzenAdjDlls">Path to the directory containing the DLL files</param>
    public static void InitializeRyzenAdj(string pathToRyzenAdjDlls)
    {
      // Required library files
      string[] requiredFiles = { "libryzenadj.dll", "WinRing0x64.dll", "WinRing0x64.sys", "inpoutx64.dll" };

      // Check if all required files exist
      foreach (string fileName in requiredFiles)
      {
        string filePath = Path.Combine(pathToRyzenAdjDlls, fileName);
        if (!File.Exists(filePath))
        {
          throw new FileNotFoundException($"Required library file not found: {filePath}");
        }
      }

      Logger.WriteLine("Calling init_ryzenadj()...");
      ry = init_ryzenadj();

      if (ry == IntPtr.Zero)
      {
        throw new Exception($"RyzenAdj could not get initialized.");
      }
      Logger.WriteLine("RyzenAdj initialized successfully!");
    }

    /// <summary>
    /// Invoke the appropriate RyzenAdj adjustment method based on field name
    /// </summary>
    /// <param name="fieldName">The name of the field to adjust</param>
    /// <param name="value">The value to set</param>
    /// <returns>Result code from the RyzenAdj library</returns>
    public static int InvokeAdjustMethod(string fieldName, uint value)
    {
      switch (fieldName)
      {
        case "stapm_limit": return set_stapm_limit(ry, value);
        case "fast_limit": return set_fast_limit(ry, value);
        case "slow_limit": return set_slow_limit(ry, value);
        case "slow_time": return set_slow_time(ry, value);
        case "stapm_time": return set_stapm_time(ry, value);
        case "tctl_temp": return set_tctl_temp(ry, value);
        case "vrm_current": return set_vrm_current(ry, value);
        case "vrmsoc_current": return set_vrmsoc_current(ry, value);
        case "vrmmax_current": return set_vrmmax_current(ry, value);
        case "vrmsocmax_current": return set_vrmsocmax_current(ry, value);
        case "psi0_current": return set_psi0_current(ry, value);
        case "psi0soc_current": return set_psi0soc_current(ry, value);
        case "max_gfxclk_freq": return set_max_gfxclk_freq(ry, value);
        case "min_gfxclk_freq": return set_min_gfxclk_freq(ry, value);
        case "max_socclk_freq": return set_max_socclk_freq(ry, value);
        case "min_socclk_freq": return set_min_socclk_freq(ry, value);
        case "max_fclk_freq": return set_max_fclk_freq(ry, value);
        case "min_fclk_freq": return set_min_fclk_freq(ry, value);
        case "max_vcn": return set_max_vcn(ry, value);
        case "min_vcn": return set_min_vcn(ry, value);
        case "max_lclk": return set_max_lclk(ry, value);
        case "prochot_deassertion_ramp": return set_prochot_deassertion_ramp(ry, value);
        case "apu_skin_temp_limit": return set_apu_skin_temp_limit(ry, value);
        case "dgpu_skin_temp_limit": return set_dgpu_skin_temp_limit(ry, value);
        case "apu_slow_limit": return set_apu_slow_limit(ry, value);
        default: return -999;
      }
    }

    /// <summary>
    /// Get the current monitor value for a specific field
    /// </summary>
    /// <param name="monitorField">The field name to monitor</param>
    /// <returns>Current value of the field</returns>
    public static float GetMonitorValue(string monitorField)
    {
      if (string.IsNullOrEmpty(monitorField))
        return 0;

      switch (monitorField)
      {
        case "stapm_limit": return get_stapm_limit(ry);
        case "fast_limit": return get_fast_limit(ry);
        case "slow_limit": return get_slow_limit(ry);
        case "stapm_value": return get_stapm_value(ry);
        case "stapm_time": return get_stapm_time(ry);
        case "fast_value": return get_fast_value(ry);
        case "slow_value": return get_slow_value(ry);
        case "apu_slow_limit": return get_apu_slow_limit(ry);
        case "apu_slow_value": return get_apu_slow_value(ry);
        case "vrm_current": return get_vrm_current(ry);
        case "vrm_current_value": return get_vrm_current_value(ry);
        case "vrmsoc_current": return get_vrmsoc_current(ry);
        case "vrmsoc_current_value": return get_vrmsoc_current_value(ry);
        case "vrmmax_current": return get_vrmmax_current(ry);
        case "vrmmax_current_value": return get_vrmmax_current_value(ry);
        case "vrmsocmax_current": return get_vrmsocmax_current(ry);
        case "vrmsocmax_current_value": return get_vrmsocmax_current_value(ry);
        case "tctl_temp": return get_tctl_temp(ry);
        case "tctl_temp_value": return get_tctl_temp_value(ry);
        case "apu_skin_temp_limit": return get_apu_skin_temp_limit(ry);
        case "apu_skin_temp_value": return get_apu_skin_temp_value(ry);
        case "dgpu_skin_temp_limit": return get_dgpu_skin_temp_limit(ry);
        case "dgpu_skin_temp_value": return get_dgpu_skin_temp_value(ry);
        default: return 0;
      }
    }

    /// <summary>
    /// Refresh the RyzenAdj table (wrapper for external use)
    /// </summary>
    public static void RefreshTable()
    {
      refresh_table(ry);
    }
  }
}