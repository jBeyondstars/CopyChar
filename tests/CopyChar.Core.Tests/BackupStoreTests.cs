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
        Assert.Equal(character.FolderPath, backup.FolderPath);
        Assert.Equal("Alpha (Realm)", backup.Label);
    }

    [Fact]
    public void Restore_puts_the_folder_back_as_it_was_backed_up()
    {
        using var wtf = new TempWtf();
        var config = wtf.AddCharacterFile("111#1", "Realm", "Alpha", "config-cache.wtf", "before");
        var character = wtf.Character("Alpha");
        var store = new BackupStore(Path.Combine(wtf.Root, "backups"));
        var backup = store.Create(character);
        File.WriteAllText(config, "after");
        var added = wtf.AddCharacterFile("111#1", "Realm", "Alpha", @"SavedVariables\TomTom.lua");

        store.Restore(backup);

        Assert.Equal("before", File.ReadAllText(config));
        Assert.False(File.Exists(added));
        Assert.Equal([config], Directory.GetFiles(character.FolderPath, "*", SearchOption.AllDirectories));
    }

    [Fact]
    public void Restore_backs_up_the_current_state_first()
    {
        using var wtf = new TempWtf();
        var config = wtf.AddCharacterFile("111#1", "Realm", "Alpha", "config-cache.wtf", "before");
        var store = new BackupStore(Path.Combine(wtf.Root, "backups"));
        var backup = store.Create(wtf.Character("Alpha"));
        File.WriteAllText(config, "after");

        var current = store.Restore(backup)!;
        store.Restore(current);

        Assert.Equal("after", File.ReadAllText(config));
    }

    [Fact]
    public void Account_restore_only_touches_files_at_the_root_of_the_account()
    {
        using var wtf = new TempWtf();
        var bindings = wtf.AddAccountFile("111#1", "bindings-cache.wtf", "before");
        var characterConfig = wtf.AddCharacterFile("111#1", "Realm", "Alpha", "config-cache.wtf", "before");
        var store = new BackupStore(Path.Combine(wtf.Root, "backups"));
        var backup = store.CreateForAccount(wtf.AccountPath("111#1"));
        File.WriteAllText(bindings, "after");
        var added = wtf.AddAccountFile("111#1", "macros-cache.txt");
        File.WriteAllText(characterConfig, "after");

        store.Restore(backup);

        Assert.Equal("before", File.ReadAllText(bindings));
        Assert.False(File.Exists(added));
        Assert.Equal("after", File.ReadAllText(characterConfig));
        Assert.Equal([bindings], Directory.GetFiles(wtf.AccountPath("111#1")));
        Assert.Equal("Account 111#1", backup.Label);
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
