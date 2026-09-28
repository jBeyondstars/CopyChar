using System.IO.Compression;

namespace CopyChar.Core;

// A character backup holds the whole character folder. An account backup only holds the files
// at the root of the account folder: its realm and character folders have backups of their own.
public sealed record Backup(string ZipPath, string FolderPath, bool IsAccount, DateTime CreatedAt)
{
    public string Label => IsAccount
        ? $"Account {Path.GetFileName(FolderPath)}"
        : $"{Path.GetFileName(FolderPath)} ({Path.GetFileName(Path.GetDirectoryName(FolderPath))})";
}

// One zip per folder and per copy. The original folder path is stored inside the zip
// so a backup can be restored without the user pointing at the right folder.
public sealed class BackupStore(string rootPath)
{
    private const string CharacterOriginEntry = ".copychar-origin";
    private const string AccountOriginEntry = ".copychar-account-origin";

    public Backup Create(Character character)
    {
        var zipPath = NewZipPath($"{character.Account}_{character.Realm}_{character.Name}");

        ZipFile.CreateFromDirectory(character.FolderPath, zipPath);
        using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
        {
            WriteOrigin(archive, CharacterOriginEntry, character.FolderPath);
        }

        return Read(zipPath);
    }

    public Backup CreateForAccount(string accountFolder)
    {
        var zipPath = NewZipPath(Path.GetFileName(accountFolder));

        using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            foreach (var file in Directory.EnumerateFiles(accountFolder))
                archive.CreateEntryFromFile(file, Path.GetFileName(file));
            WriteOrigin(archive, AccountOriginEntry, accountFolder);
        }

        return Read(zipPath);
    }

    // Puts the folder back exactly as it was, including removing files added since.
    // The current state is backed up first so a restore can itself be undone.
    public Backup? Restore(Backup backup)
    {
        Backup? current = null;
        if (Directory.Exists(backup.FolderPath))
        {
            if (backup.IsAccount)
            {
                current = CreateForAccount(backup.FolderPath);
                foreach (var file in Directory.EnumerateFiles(backup.FolderPath))
                    File.Delete(file);
            }
            else
            {
                current = Create(Character.FromFolder(backup.FolderPath));
                Directory.Delete(backup.FolderPath, recursive: true);
            }
        }

        ZipFile.ExtractToDirectory(backup.ZipPath, backup.FolderPath);
        File.Delete(Path.Combine(backup.FolderPath, backup.IsAccount ? AccountOriginEntry : CharacterOriginEntry));
        return current;
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

    private string NewZipPath(string name)
    {
        Directory.CreateDirectory(rootPath);
        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff");
        return Path.Combine(rootPath, $"{stamp}_{name}.zip");
    }

    private static void WriteOrigin(ZipArchive archive, string entryName, string folderPath)
    {
        using var writer = new StreamWriter(archive.CreateEntry(entryName).Open());
        writer.Write(folderPath);
    }

    private static Backup Read(string zipPath)
    {
        using var archive = ZipFile.OpenRead(zipPath);
        var isAccount = archive.GetEntry(AccountOriginEntry) is not null;
        var origin = archive.GetEntry(isAccount ? AccountOriginEntry : CharacterOriginEntry)
            ?? throw new InvalidDataException($"{zipPath} is not a CopyChar backup.");
        using var reader = new StreamReader(origin.Open());
        return new Backup(zipPath, reader.ReadToEnd(), isAccount, File.GetLastWriteTime(zipPath));
    }
}
