using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CopyChar.App;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

// Wraps an item shown with a checkbox.
public sealed class Selectable<T>(T item, bool isSelected = false) : ObservableObject
{
    public T Item { get; } = item;

    public bool IsSelected
    {
        get;
        set { field = value; OnPropertyChanged(); }
    } = isSelected;
}
