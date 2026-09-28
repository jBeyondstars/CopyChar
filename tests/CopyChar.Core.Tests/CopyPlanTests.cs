namespace CopyChar.Core.Tests;

public class CopyPlanTests
{
    [Fact]
    public void Keeps_files_present_in_source_and_reports_the_others_as_skipped()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Target", "config-cache.wtf");
        var source = wtf.Character("Source");
        var target = wtf.Character("Target");

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
        var source = wtf.Character("Source");
        var target = wtf.Character("Target");

        var plan = CopyPlan.Create(source, [target], [], ["TomTom"]);

        Assert.Equal([@"SavedVariables\TomTom.lua"], plan.Files);
    }

    [Fact]
    public void Rejects_the_source_character_as_a_target()
    {
        using var wtf = new TempWtf();
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Target", "config-cache.wtf");
        var source = wtf.Character("Source");
        var target = wtf.Character("Target");

        Assert.Throws<ArgumentException>(() =>
            CopyPlan.Create(source, [target, source], SettingCategory.PerCharacter, []));
    }

    [Fact]
    public void Sends_account_settings_only_to_targets_on_another_account()
    {
        using var wtf = new TempWtf();
        wtf.AddAccountFile("111#1", "bindings-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "SameAccount", "config-cache.wtf");
        wtf.AddCharacterFile("111#2", "Realm", "OtherAccount", "config-cache.wtf");
        wtf.AddCharacterFile("111#2", "Realm", "OtherAccountToo", "config-cache.wtf");

        var plan = CopyPlan.Create(
            wtf.Character("Source"),
            [wtf.Character("SameAccount"), wtf.Character("OtherAccount"), wtf.Character("OtherAccountToo")],
            [SettingCategory.AccountKeyBindings, SettingCategory.AccountMacros],
            []);

        Assert.Equal([wtf.AccountPath("111#2")], plan.TargetAccounts);
        Assert.Equal(["bindings-cache.wtf"], plan.AccountFiles);
        Assert.Equal(["macros-cache.txt (account)"], plan.SkippedFiles);
        Assert.Empty(plan.Files);
    }

    [Fact]
    public void Ignores_account_settings_when_every_target_shares_the_source_account()
    {
        using var wtf = new TempWtf();
        wtf.AddAccountFile("111#1", "bindings-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Source", "config-cache.wtf");
        wtf.AddCharacterFile("111#1", "Realm", "Target", "config-cache.wtf");

        var plan = CopyPlan.Create(
            wtf.Character("Source"), [wtf.Character("Target")], [SettingCategory.AccountKeyBindings], []);

        Assert.Empty(plan.TargetAccounts);
        Assert.Empty(plan.AccountFiles);
        Assert.Empty(plan.SkippedFiles);
    }
}
