using System.ComponentModel;

public class CircuitItem : INotifyPropertyChanged
{
    private bool _isChecked;
    private string _CircuitNumber;
    private string _Name;

    public string Name
    {
        get => _Name;
        set
        {
            if (_Name != value)
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public string CircuitNumber
    {
        get => _CircuitNumber;
        set
        {
            if (_CircuitNumber != value)
            {
                _CircuitNumber = value;
                OnPropertyChanged(nameof(CircuitNumber));
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public string DisplayName => $"{CircuitNumber} {Name}";

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}