// Monitor state class to encapsulate monitoring functionality
namespace RyzenAdjService
{
  public class FieldMonitorState
  {
    public string FieldName { get; }
    public double Target { get; set; }
    public double Epsilon { get; set; }

    public FieldMonitorState(string fieldName, double epsilon)
    {
      FieldName = fieldName;
      Target = 0;
      Epsilon = epsilon;
    }
  }
}