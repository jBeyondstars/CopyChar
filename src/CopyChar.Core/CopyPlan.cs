namespace CopyChar.Core;

// Files are relative to a character folder, so the same list applies to every target.
// AccountFiles are relative to an account folder and only go to TargetAccounts: the accounts of
// the targets other than the source account, whose characters already share these files.
// SkippedFiles were selected but do not exist in the source folders.
public sealed record CopyPlan(
    Character Source,
    IReadOnlyList<Character> Targets,
    IReadOnlyList<string> Files,
    IReadOnlyList<string> TargetAccounts,
    IReadOnlyList<string> AccountFiles,
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

        var categoryList = categories.ToList();
        var characterFiles = categoryList
            .Where(c => !c.AccountWide)
            .SelectMany(c => c.Files)
            .Concat(addons.Select(AddonSavedVariables.FileName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var accountFiles = categoryList
            .Where(c => c.AccountWide)
            .SelectMany(c => c.Files)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var otherAccounts = targetList
            .Select(t => t.AccountFolder)
            .Where(a => !a.Equals(source.AccountFolder, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (otherAccounts.Count == 0)
            accountFiles = [];

        var existing = characterFiles.Where(f => File.Exists(Path.Combine(source.FolderPath, f))).ToList();
        var existingAccount = accountFiles.Where(f => File.Exists(Path.Combine(source.AccountFolder, f))).ToList();
        var skipped = characterFiles.Except(existing)
            .Concat(accountFiles.Except(existingAccount).Select(f => $"{f} (account)"))
            .ToList();

        return new CopyPlan(
            source,
            targetList,
            existing,
            existingAccount.Count == 0 ? [] : otherAccounts,
            existingAccount,
            skipped);
    }
}
