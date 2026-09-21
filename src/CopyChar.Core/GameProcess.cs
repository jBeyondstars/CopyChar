using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CopyChar.Core;

// The client rewrites the WTF files of the logged character when it logs out or exits,
// which would silently undo a copy made while it is running.
public static partial class GameProcess
{
    // Wow, WowClassic, and their B (beta, such as WoW Forever) and T (PTR) builds.
    [GeneratedRegex(@"^Wow(Classic)?[BT]?$", RegexOptions.IgnoreCase)]
    private static partial Regex ClientName();

    public static bool IsClientName(string processName) => ClientName().IsMatch(processName);

    // True when a client runs from the folder containing path (a flavor folder or anything below it).
    public static bool IsRunning(string path)
    {
        var target = WithTrailingSeparator(path);

        foreach (var process in Process.GetProcesses())
        {
            using (process)
            {
                if (!IsClientName(process.ProcessName))
                    continue;

                try
                {
                    var exe = process.MainModule?.FileName;
                    if (exe is null || target.StartsWith(WithTrailingSeparator(Path.GetDirectoryName(exe)!), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                catch (Win32Exception)
                {
                    // Path not readable (access denied): assume it is this install rather than risk it.
                    return true;
                }
            }
        }

        return false;
    }

    private static string WithTrailingSeparator(string path) =>
        Path.TrimEndingDirectorySeparator(path) + Path.DirectorySeparatorChar;
}
