namespace CopyChar.Core.Tests;

public class WtfScannerTests
{
    [Fact]
    public void Finds_characters_of_every_account_and_realm()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm A", "Alpha", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm B", "Beta", "AddOns.txt");
        wtf.AddCharacterFile("222#1", "Realm A", "Gamma", "config-cache.wtf");

        var characters = WtfScanner.FindCharacters(wtf.WtfPath);

        Assert.Equal(
            ["111#1/Realm A/Alpha", "111#1/Realm B/Beta", "222#1/Realm A/Gamma"],
            characters.Select(c => $"{c.Account}/{c.Realm}/{c.Name}"));
        Assert.Equal(wtf.CharacterPath("111#1", "Realm A", "Alpha"), characters[0].FolderPath);
    }

    [Fact]
    public void Ignores_SavedVariables_folders_at_every_level()
    {
        using var wtf = new TempWtf();
        wtf.AddFile(@"WTF\Account\SavedVariables\Global.lua");
        wtf.AddFile(@"WTF\Account\111#1\SavedVariables\Addon.lua");
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", @"SavedVariables\Addon.lua");

        var characters = WtfScanner.FindCharacters(wtf.WtfPath);

        Assert.Equal("Alpha", Assert.Single(characters).Name);
    }

    [Fact]
    public void Returns_nothing_when_no_account_folder_exists()
    {
        using var wtf = new TempWtf();
        wtf.AddFile(@"WTF\Config.wtf");

        Assert.Empty(WtfScanner.FindCharacters(wtf.WtfPath));
    }
}
