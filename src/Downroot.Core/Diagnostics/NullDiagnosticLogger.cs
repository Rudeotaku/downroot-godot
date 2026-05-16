namespace Downroot.Core.Diagnostics;

public sealed class NullDiagnosticLogger : IDiagnosticLogger
{
    public static NullDiagnosticLogger Instance { get; } = new();

    private NullDiagnosticLogger()
    {
    }

    public void Log(string message)
    {
    }
}
