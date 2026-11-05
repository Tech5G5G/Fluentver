using System.ComponentModel;

namespace Fluver.Contracts;

public interface ISetting<T> : INotifyPropertyChanged
{
    T Value { get; set; }

    event EventHandler<T> ValueChanged;
}
