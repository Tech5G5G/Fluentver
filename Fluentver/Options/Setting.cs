using System.ComponentModel;
using Windows.Storage;

namespace Fluver.Options;

public partial class Setting<T>(string key, T defaultValue) : ISetting<T>
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

public sealed partial class EnumSetting<T>(string key, T defaultValue) : Setting<T>(key, defaultValue) where T : struct, Enum
{
    protected override object Serialize(T value)
    {
        return (int)(object)value;
    }
}
