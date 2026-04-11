using System.IO.Compression;

namespace CopyChar.Core;

public sealed record Backup(string ZipPath, string CharacterFolder, DateTime CreatedAt)
{
    public string CharacterLabel =>
        $"{Path.GetFileName(CharacterFolder)} ({Path.GetFileName(Path.GetDirectoryName(CharacterFolder))})";
}

// One zip per character folder and per copy. The original folder path is stored inside
// the zip so a backup can be restored without the user pointing at the right folder.
public sealed class BackupStore(string rootPath)
{
    private const string OriginEntry = ".copychar-origin";

    public Backup Create(Character character)
    {
        Directory.CreateDirectory(rootPath);
        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff");
        var zipPath = Path.Combine(rootPath, $"{stamp}_{character.Account}_{character.Realm}_{character.Name}.zip");

        ZipFile.CreateFromDirectory(character.FolderPath, zipPath);
        using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
        using (var writer = new StreamWriter(archive.CreateEntry(OriginEntry).Open()))
        {
            writer.Write(character.FolderPath);
        }

        return Read(zipPath);
    }

    public IReadOnlyList<Backup> List()
    {
        if (!Directory.Exists(rootPath))
            return [];

        return Directory.EnumerateFiles(rootPath, "*.zip")
            .Select(Read)
            .OrderByDescending(b => b.ZipPath, StringComparer.Ordinal)
            .ToList();
    }

    private static Backup Read(string zipPath)
    {
        using var archive = ZipFile.OpenRead(zipPath);
        var origin = archive.GetEntry(OriginEntry)
            ?? throw new InvalidDataException($"{zipPath} is not a CopyChar backup.");
        using var reader = new StreamReader(origin.Open());
        return new Backup(zipPath, reader.ReadToEnd(), File.GetLastWriteTime(zipPath));
    }
}
