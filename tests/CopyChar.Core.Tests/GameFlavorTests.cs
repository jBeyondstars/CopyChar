namespace CopyChar.Core.Tests;

public class GameFlavorTests
{
    [Fact]
    public void Finds_flavor_folders_that_contain_a_WTF_folder()
    {
        using var wtf = new TempWtf();
        wtf.AddFile(@"_retail_\WTF\Config.wtf");
        wtf.AddFile(@"_anniversary_\WTF\Config.wtf");
        wtf.AddFile(@"Data\data.000");

        var flavors = GameFlavor.FindAll(wtf.Root);

        Assert.Equal(["anniversary", "retail"], flavors.Select(f => f.Name));
        Assert.Equal(Path.Combine(wtf.Root, "_retail_", "WTF"), flavors[1].WtfPath);
    }

    [Fact]
    public void Accepts_a_flavor_folder_picked_directly()
    {
        using var wtf = new TempWtf();
        wtf.AddFile(@"_classic_\WTF\Config.wtf");

        var flavor = Assert.Single(GameFlavor.FindAll(Path.Combine(wtf.Root, "_classic_")));

        Assert.Equal("classic", flavor.Name);
    }
}
