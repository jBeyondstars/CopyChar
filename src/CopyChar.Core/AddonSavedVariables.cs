namespace CopyChar.Core;

// Per-character addon settings live in <character>/SavedVariables/<Addon>.lua.
// The client also writes a .lua.bak next to each file; only the .lua is read at login.
public static class AddonSavedVariables
{
    public const string FolderName = "SavedVariables";

    public static IReadOnlyList<string> FindAddons(Character character)
    {
        var folder = Path.Combine(character.FolderPath, FolderName);
        if (!Directory.Exists(folder))
            return [];

        return Directory.EnumerateFiles(folder, "*.lua")
            .Select(Path.GetFileNameWithoutExtension)
            .OfType<string>()
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string FileName(string addon) => Path.Combine(FolderName, addon + ".lua");
}
