using System.IO;

namespace CopyChar.App;

// Everything the app writes outside the game folder lives under %LOCALAPPDATA%\CopyChar.
public static class AppStorage
{
    private static readonly string Root =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CopyChar");

    private static readonly string LastInstallFile = Path.Combine(Root, "last-install.txt");

    public static string BackupFolder { get; } = Path.Combine(Root, "backups");

    // Blizzard does not register the game path anywhere reliable, so fall back to the default folders.
    public static string GuessInstallPath()
    {
        if (File.Exists(LastInstallFile))
        {
            var last = File.ReadAllText(LastInstallFile).Trim();
            if (Directory.Exists(last))
                return last;
        }

        string[] candidates =
        [
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "World of Warcraft"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "World of Warcraft"),
        ];
        return candidates.FirstOrDefault(Directory.Exists) ?? "";
    }

    public static void SaveInstallPath(string path)
    {
        Directory.CreateDirectory(Root);
        File.WriteAllText(LastInstallFile, path);
    }
}
