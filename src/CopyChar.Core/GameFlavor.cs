namespace CopyChar.Core;

// A game client folder such as _retail_, _classic_ or _classic_era_, each with its own WTF.
public sealed record GameFlavor(string Name, string FolderPath)
{
    public string WtfPath => Path.Combine(FolderPath, "WTF");

    public override string ToString() => Name;

    // Accepts either the install root (World of Warcraft) or a flavor folder picked directly.
    public static IReadOnlyList<GameFlavor> FindAll(string path)
    {
        if (!Directory.Exists(path))
            return [];

        if (HasWtf(path))
            return [FromFolder(path)];

        return Directory.EnumerateDirectories(path)
            .Where(HasWtf)
            .Select(FromFolder)
            .OrderBy(f => f.Name)
            .ToList();
    }

    private static bool HasWtf(string folder) => Directory.Exists(Path.Combine(folder, "WTF"));

    private static GameFlavor FromFolder(string folder) =>
        new(Path.GetFileName(folder).Trim('_'), folder);
}
