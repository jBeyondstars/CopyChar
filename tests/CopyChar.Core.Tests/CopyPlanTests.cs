namespace CopyChar.Core.Tests;

public class CopyPlanTests
{
    [Fact]
    public void Keeps_files_present_in_source_and_reports_the_others_as_skipped()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Target", "config-cache.wtf");
        var (source, target) = Characters(wtf);

        var plan = CopyPlan.Create(source, [target], [SettingCategory.GameOptions, SettingCategory.Macros], []);

        Assert.Equal(["config-cache.wtf"], plan.Files);
        Assert.Equal(["macros-cache.txt"], plan.SkippedFiles);
    }

    [Fact]
    public void Includes_saved_variables_of_selected_addons_only()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Source", @"SavedVariables\Questie.lua");
        wtf.AddCharacterFile("111#1", "Realm", "Source", @"SavedVariables\TomTom.lua");
        wtf.AddCharacterFile("111#1", "Realm", "Target", "AddOns.txt");
        var (source, target) = Characters(wtf);

        var plan = CopyPlan.Create(source, [target], [], ["TomTom"]);

        Assert.Equal([@"SavedVariables\TomTom.lua"], plan.Files);
    }

    [Fact]
    public void Rejects_the_source_character_as_a_target()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Target", "config-cache.wtf");
        var (source, target) = Characters(wtf);

        Assert.Throws<ArgumentException>(() =>
            CopyPlan.Create(source, [target, source], SettingCategory.All, []));
    }

    private static (Character Source, Character Target) Characters(TempWtf wtf)
    {
        var characters = WtfScanner.FindCharacters(wtf.WtfPath);
        return (characters.Single(c => c.Name == "Source"), characters.Single(c => c.Name == "Target"));
    }
}
