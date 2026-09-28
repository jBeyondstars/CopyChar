using System.IO.Compression;

namespace CopyChar.Core.Tests;

public class CharacterCopierTests
{
    [Fact]
    public void Copies_planned_files_and_leaves_target_cache_md5_untouched()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf", "source config");
        wtf.AddCharacterFile("111#1", "Realm", "Source", @"SavedVariables\TomTom.lua", "source tomtom");
        wtf.AddCharacterFile("111#1", "Realm", "Source", "cache.md5", "source md5");
        var targetConfig = wtf.AddCharacterFile("111#1", "Realm", "Target", "config-cache.wtf", "target config");
        var targetMd5 = wtf.AddCharacterFile("111#1", "Realm", "Target", "cache.md5", "target md5");
        var source = wtf.Character("Source");
        var target = wtf.Character("Target");
        var plan = CopyPlan.Create(source, [target], [SettingCategory.GameOptions], ["TomTom"]);

        new CharacterCopier(new BackupStore(Path.Combine(wtf.Root, "backups"))).Execute(plan);

        Assert.Equal("source config", File.ReadAllText(targetConfig));
        Assert.Equal("source tomtom", File.ReadAllText(Path.Combine(target.FolderPath, @"SavedVariables\TomTom.lua")));
        Assert.Equal("target md5", File.ReadAllText(targetMd5));
    }

    [Fact]
    public void Backs_up_each_target_before_overwriting_it()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf", "source config");
        wtf.AddCharacterFile("111#1", "Realm", "Target", "config-cache.wtf", "target config");
        var source = wtf.Character("Source");
        var target = wtf.Character("Target");
        var plan = CopyPlan.Create(source, [target], [SettingCategory.GameOptions], []);

        var backup = Assert.Single(new CharacterCopier(new BackupStore(Path.Combine(wtf.Root, "backups"))).Execute(plan));

        using var archive = ZipFile.OpenRead(backup.ZipPath);
        using var reader = new StreamReader(archive.GetEntry("config-cache.wtf")!.Open());
        Assert.Equal("target config", reader.ReadToEnd());
        Assert.Equal(target.FolderPath, backup.FolderPath);
    }

    [Fact]
    public void Copies_account_files_to_the_target_account_after_backing_it_up()
    {
        using var wtf = new TempWtf();
        wtf.AddAccountFile("111#1", "bindings-cache.wtf", "source bindings");
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf", "source config");
        var targetBindings = wtf.AddAccountFile("111#2", "bindings-cache.wtf", "target bindings");
        var targetMd5 = wtf.AddAccountFile("111#2", "cache.md5", "target md5");
        wtf.AddCharacterFile("111#2", "Realm", "Target", "config-cache.wtf", "target config");
        var plan = CopyPlan.Create(
            wtf.Character("Source"), [wtf.Character("Target")], [SettingCategory.AccountKeyBindings], []);

        var backups = new CharacterCopier(new BackupStore(Path.Combine(wtf.Root, "backups"))).Execute(plan);

        Assert.Equal("source bindings", File.ReadAllText(targetBindings));
        Assert.Equal("target md5", File.ReadAllText(targetMd5));
        var backup = Assert.Single(backups);
        Assert.True(backup.IsAccount);
        using var archive = ZipFile.OpenRead(backup.ZipPath);
        using var reader = new StreamReader(archive.GetEntry("bindings-cache.wtf")!.Open());
        Assert.Equal("target bindings", reader.ReadToEnd());
    }
}
