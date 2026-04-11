using System.IO.Compression;

namespace CopyChar.Core.Tests;

public class BackupStoreTests
{
    [Fact]
    public void Backup_contains_every_file_of_the_character_folder()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", @"SavedVariables\Questie.lua");
        var character = WtfScanner.FindCharacters(wtf.WtfPath).Single();
        var store = new BackupStore(Path.Combine(wtf.Root, "backups"));

        var backup = store.Create(character);

        using var archive = ZipFile.OpenRead(backup.ZipPath);
        var entries = archive.Entries.Select(e => e.FullName.Replace('\\', '/')).ToList();
        Assert.Contains("config-cache.wtf", entries);
        Assert.Contains("SavedVariables/Questie.lua", entries);
        Assert.Equal(character.FolderPath, backup.CharacterFolder);
        Assert.Equal("Alpha (Realm)", backup.CharacterLabel);
    }

    [Fact]
    public void Lists_backups_newest_first()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", "config-cache.wtf");
        var character = WtfScanner.FindCharacters(wtf.WtfPath).Single();
        var store = new BackupStore(Path.Combine(wtf.Root, "backups"));

        var first = store.Create(character);
        var second = store.Create(character);

        Assert.Equal([second.ZipPath, first.ZipPath], store.List().Select(b => b.ZipPath));
    }
}
