namespace CopyChar.Core;

// Layout: WTF/Account/<account>/<realm>/<character>/
// Each level can also hold a SavedVariables folder that is not an account, realm or character.
public static class WtfScanner
{
    private const string SavedVariablesFolder = "SavedVariables";

    public static IReadOnlyList<Character> FindCharacters(string wtfPath)
    {
        var accountsPath = Path.Combine(wtfPath, "Account");
        if (!Directory.Exists(accountsPath))
            return [];

        return SubFolders(accountsPath)
            .SelectMany(SubFolders)
            .SelectMany(SubFolders)
            .Select(Character.FromFolder)
            .OrderBy(c => c.Account)
            .ThenBy(c => c.Realm)
            .ThenBy(c => c.Name)
            .ToList();
    }

    private static IEnumerable<string> SubFolders(string path) =>
        Directory.EnumerateDirectories(path)
            .Where(d => !Path.GetFileName(d).Equals(SavedVariablesFolder, StringComparison.OrdinalIgnoreCase));
}
