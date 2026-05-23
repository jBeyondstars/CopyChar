using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CopyChar.Core;

// The client rewrites the WTF files of the logged character when it logs out or exits,
// which would silently undo a copy made while it is running.
public static partial class GameProcess
{
    // Wow for retail, WowClassic for every Classic flavor.
    [GeneratedRegex(@"^Wow(Classic)?$", RegexOptions.IgnoreCase)]
    private static partial Regex ClientName();

    public static bool IsClientName(string processName) => ClientName().IsMatch(processName);

    public static bool IsRunning(GameFlavor flavor)
    {
        var flavorFolder = flavor.FolderPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

        foreach (var process in Process.GetProcesses())
        {
            using (process)
            {
                if (!IsClientName(process.ProcessName))
                    continue;

                try
                {
                    var exe = process.MainModule?.FileName;
                    if (exe is null || exe.StartsWith(flavorFolder, StringComparison.OrdinalIgnoreCase))
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
}
