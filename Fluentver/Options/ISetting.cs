using System.ComponentModel;

namespace Fluver.Options;

public interface ISetting<T> : INotifyPropertyChanged
{
    T Value { get; set; }

    event EventHandler<T> ValueChanged;
}
