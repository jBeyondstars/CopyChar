using System.IO;
using CopyChar.Core;

namespace CopyChar.App;

public sealed class MainViewModel : ObservableObject
{
    public MainViewModel(string installPath)
    {
        InstallPath = installPath;
    }

    public string InstallPath
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            Flavors = GameFlavor.FindAll(value);
            // Config.wtf is rewritten on every exit, so the newest one is the flavor played last.
            SelectedFlavor = Flavors.MaxBy(f => File.GetLastWriteTime(Path.Combine(f.WtfPath, "Config.wtf")));
        }
    } = "";

    public IReadOnlyList<GameFlavor> Flavors
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = [];

    public GameFlavor? SelectedFlavor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            Characters = value is null ? [] : WtfScanner.FindCharacters(value.WtfPath);
            Source = null;
        }
    }

    public IReadOnlyList<Character> Characters
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = [];

    public Character? Source
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            Targets = Characters
                .Where(c => value is not null && c != value)
                .Select(c => new Selectable<Character>(c))
                .ToList();
        }
    }

    public IReadOnlyList<Selectable<Character>> Targets
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = [];
}
