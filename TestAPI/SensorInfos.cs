using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestAPI;

public class SensorInfos : INotifyPropertyChanged
{
    private bool _isChecked;
    public string Name { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if(value != _isChecked)
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }
    }

    public SensorInfos(string name, bool isChecked)
    {
        Name = name;
        IsChecked = isChecked;
    }


    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
