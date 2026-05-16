using Downroot.Core.Diagnostics;
using Godot;

namespace Downroot.Game.Infrastructure;

public sealed class GodotDiagnosticLogger : IDiagnosticLogger
{
    public void Log(string message)
    {
        GD.Print(message);
    }
}
