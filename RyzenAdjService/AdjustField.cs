using System;
using System.Linq;
using RyzenAdjService;

namespace RyzenAdjService
{
  public static class AdjustField
  {
    public static void Adjust(string fieldName, uint value, FieldMonitorState[] fieldMonitorStates)
    {
      var monitorState = fieldMonitorStates.FirstOrDefault(m => m.FieldName == fieldName);
      if (monitorState != null)
      {
        if (value > 2000)
        {
          // values are set like 28000 for 28W, but monitor values are like 28.0
          monitorState.Target = value / 1000;
        }
        else
        {
          monitorState.Target = value;
        }
      }

      int res = LibRyzenAdj.InvokeAdjustMethod(fieldName, value);
      switch (res)
      {
        case 0:
          Logger.WriteLine($"set {fieldName} to {value}");
          return;
        case -1:
          Logger.WriteError($"set_{fieldName} is not supported on this family");
          break;
        case -3:
          Logger.WriteError($"set_{fieldName} is not supported on this SMU");
          break;
        case -4:
          Logger.WriteError($"set_{fieldName} is rejected by SMU");
          break;
        default:
          Logger.WriteError($"set_{fieldName} did fail with {res}");
          break;
      }
    }
  }
}