namespace CopyChar.Core;

// Files are relative to a character folder, so the same list applies to every target.
// SkippedFiles were selected but do not exist in the source folder.
public sealed record CopyPlan(
    Character Source,
    IReadOnlyList<Character> Targets,
    IReadOnlyList<string> Files,
    IReadOnlyList<string> SkippedFiles)
{
    public static CopyPlan Create(
        Character source,
        IEnumerable<Character> targets,
        IEnumerable<SettingCategory> categories,
        IEnumerable<string> addons)
    {
        var targetList = targets.ToList();
        if (targetList.Count == 0)
            throw new ArgumentException("No target character selected.", nameof(targets));
        if (targetList.Contains(source))
            throw new ArgumentException("The source character cannot also be a target.", nameof(targets));

        var selected = categories
            .SelectMany(c => c.Files)
            .Concat(addons.Select(AddonSavedVariables.FileName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var existing = selected.Where(f => File.Exists(Path.Combine(source.FolderPath, f))).ToList();
        var skipped = selected.Except(existing).ToList();

        return new CopyPlan(source, targetList, existing, skipped);
    }
}
