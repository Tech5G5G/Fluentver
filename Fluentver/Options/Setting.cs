using System.ComponentModel;
using Windows.Storage;

namespace Fluver.Options;

public partial class Setting<T>(string key, Func<T> defaultFactory) : INotifyPropertyChanged
{
    private static readonly ApplicationDataContainer s_store = ApplicationData.Current.LocalSettings;

    public event EventHandler<T> ValueChanged;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event PropertyChangedEventHandler PropertyChanged;

    public T Value
    {
        get
        {
            if (s_store.Values.TryGetValue(key, out object value))
            {
                return Deserialize(value);
            }
            else
            {
                var defaultValue = defaultFactory();
                s_store.Values[key] = Serialize(defaultValue);
                return defaultValue;
            }
        }
        set
        {
            s_store.Values[key] = Serialize(value);

            ValueChanged?.Invoke(sender: this, e: value);
            PropertyChanged?.Invoke(sender: this, e: new(nameof(Value)));
        }
    }

    #region Converters

    protected virtual object Serialize(T value)
    {
        return value;
    }

    protected virtual T Deserialize(object obj)
    {
        return (T)obj;
    }

    #endregion
}

public sealed partial class EnumSetting<T>(string key, Func<T> defaultFactory) : Setting<T>(key, defaultFactory) where T : Enum
{
    protected override object Serialize(T value)
    {
        return (int)(object)value;
    }
}
