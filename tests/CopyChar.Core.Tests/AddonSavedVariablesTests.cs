namespace CopyChar.Core.Tests;

public class AddonSavedVariablesTests
{
    [Fact]
    public void Lists_addons_with_saved_variables_and_ignores_backups()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", @"SavedVariables\Questie.lua");
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", @"SavedVariables\Questie.lua.bak");
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", @"SavedVariables\auctionator.lua");
        var character = WtfScanner.FindCharacters(wtf.WtfPath).Single();

        Assert.Equal(["auctionator", "Questie"], AddonSavedVariables.FindAddons(character));
    }

    [Fact]
    public void Returns_nothing_for_a_character_without_SavedVariables_folder()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Alpha", "AddOns.txt");
        var character = WtfScanner.FindCharacters(wtf.WtfPath).Single();

        Assert.Empty(AddonSavedVariables.FindAddons(character));
    }
}
