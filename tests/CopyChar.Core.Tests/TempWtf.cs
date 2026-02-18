namespace CopyChar.Core.Tests;

// Throwaway WTF tree on disk, deleted at the end of each test.
public sealed class TempWtf : IDisposable
{
    public string Root { get; } = Directory.CreateTempSubdirectory("copychar-").FullName;

    public string WtfPath => Path.Combine(Root, "WTF");

    public string CharacterPath(string account, string realm, string name) =>
        Path.Combine(WtfPath, "Account", account, realm, name);

    public string AddFile(string relativePath, string content = "")
    {
        var path = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    public string AddCharacterFile(string account, string realm, string name, string file, string content = "")
    {
        var path = Path.Combine(CharacterPath(account, realm, name), file);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    public void Dispose() => Directory.Delete(Root, recursive: true);
}
