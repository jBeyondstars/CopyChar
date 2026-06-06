using System.Windows.Input;

namespace CopyChar.App;

public sealed class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    // WPF re-queries CanExecute on user input, which is enough for this window.
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => execute();
}
