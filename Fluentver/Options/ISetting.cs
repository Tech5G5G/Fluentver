using System.ComponentModel;

namespace Fluver.Options;

public interface ISetting<T> : INotifyPropertyChanged
{
    event EventHandler<T> ValueChanged;

    T Value { get; set; }
}
