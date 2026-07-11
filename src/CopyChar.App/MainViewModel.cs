using System.Collections.ObjectModel;
using System.IO;
using CopyChar.Core;

namespace CopyChar.App;

public sealed class MainViewModel : ObservableObject
{
    private readonly Func<string, bool> _confirm;
    private readonly BackupStore _backups = new(AppStorage.BackupFolder);

    public MainViewModel(string installPath, Func<string, bool> confirm)
    {
        _confirm = confirm;
        CopyCommand = new RelayCommand(Copy, CanCopy);
        RestoreCommand = new RelayCommand(Restore, () => SelectedBackup is not null);
        InstallPath = installPath;
        Backups = _backups.List();
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
            // Unchecked by default: some addons keep character progress (quests, guides) in these files.
            Addons = value is null
                ? []
                : AddonSavedVariables.FindAddons(value).Select(a => new Selectable<string>(a)).ToList();
        }
    }

    public IReadOnlyList<Selectable<Character>> Targets
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = [];

    public IReadOnlyList<Selectable<SettingCategory>> Categories { get; } =
        SettingCategory.All.Select(c => new Selectable<SettingCategory>(c, isSelected: true)).ToList();

    public IReadOnlyList<Selectable<string>> Addons
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = [];

    public ObservableCollection<string> Log { get; } = [];

    public RelayCommand CopyCommand { get; }

    private bool CanCopy() =>
        Source is not null
        && Targets.Any(t => t.IsSelected)
        && (Categories.Any(c => c.IsSelected) || Addons.Any(a => a.IsSelected));

    private void Copy()
    {
        if (GameProcess.IsRunning(SelectedFlavor!.FolderPath))
        {
            Write("The game is running: quit it before copying, otherwise it will overwrite the files when it closes.");
            return;
        }

        var plan = CopyPlan.Create(
            Source!,
            Selected(Targets),
            Selected(Categories),
            Selected(Addons));

        foreach (var file in plan.SkippedFiles)
            Write($"Skipped, not found for {plan.Source}: {file}");

        if (plan.Files.Count == 0)
        {
            Write("Nothing to copy.");
            return;
        }

        var question = $"Copy {plan.Files.Count} file(s) from {plan.Source} to {plan.Targets.Count} character(s)?\n\n"
            + "Each target character is backed up before being changed.";
        if (!_confirm(question))
            return;

        try
        {
            var backups = new CharacterCopier(_backups).Execute(plan);
            foreach (var (target, backup) in plan.Targets.Zip(backups))
                Write($"Copied to {target} (backup: {Path.GetFileName(backup.ZipPath)})");
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Write($"Failed: {e.Message}");
        }

        Backups = _backups.List();
    }

    public IReadOnlyList<Backup> Backups
    {
        get;
        private set { field = value; OnPropertyChanged(); }
    } = [];

    public Backup? SelectedBackup
    {
        get;
        set { field = value; OnPropertyChanged(); }
    }

    public RelayCommand RestoreCommand { get; }

    private void Restore()
    {
        var backup = SelectedBackup!;
        if (GameProcess.IsRunning(backup.CharacterFolder))
        {
            Write("The game is running: quit it before restoring, otherwise it will overwrite the files when it closes.");
            return;
        }

        var question = $"Restore {backup.CharacterLabel} to its state of {backup.CreatedAt:yyyy-MM-dd HH:mm}?\n\n"
            + "The current state is backed up first, so the restore can be undone.";
        if (!_confirm(question))
            return;

        try
        {
            var current = _backups.Restore(backup);
            Write(current is null
                ? $"Restored {backup.CharacterLabel} ({backup.CreatedAt:yyyy-MM-dd HH:mm})"
                : $"Restored {backup.CharacterLabel} ({backup.CreatedAt:yyyy-MM-dd HH:mm}), previous state backed up: {Path.GetFileName(current.ZipPath)}");
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Write($"Failed: {e.Message}");
        }

        Backups = _backups.List();
    }

    private static List<T> Selected<T>(IEnumerable<Selectable<T>> items) =>
        items.Where(i => i.IsSelected).Select(i => i.Item).ToList();

    // Newest first, so the last result is always visible without scrolling.
    private void Write(string message) => Log.Insert(0, $"{DateTime.Now:HH:mm:ss}  {message}");
}
