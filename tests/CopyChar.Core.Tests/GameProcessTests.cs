namespace CopyChar.Core.Tests;

public class GameProcessTests
{
    [Theory]
    [InlineData("Wow", true)]
    [InlineData("WowClassic", true)]
    [InlineData("WowB", true)]
    [InlineData("WowClassicT", true)]
    [InlineData("WowVoiceProxy", false)]
    [InlineData("World of Warcraft Launcher", false)]
    [InlineData("Battle.net", false)]
    public void Recognizes_game_client_process_names(string processName, bool expected)
    {
        Assert.Equal(expected, GameProcess.IsClientName(processName));
    }
}
