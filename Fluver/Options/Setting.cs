using System.ComponentModel;
using Windows.Storage;
using Windows.Foundation.Collections;

namespace Fluver.Options;

public sealed partial class Setting<T>(string key, T defaultValue, Func<T, T> coercer = null) : ISetting<T>
{
    private static readonly bool s_isEnum = typeof(T).IsEnum;

    public T Value
    {
        get
        {
            T coercedValue;

            if (LocalSettings.Values.TryGetValue(key, out object obj))
            {
                var value = (T)obj;
                SetValue(value, coercedValue = Coerce(value));
            }
            else
            {
                SetValue(coercedValue = Coerce(defaultValue));
            }

            return coercedValue;
        }
        set
        {
            var coercedValue = Coerce(value);
            if (SetValue(oldValue: Value, newValue: coercedValue))
            {
                ValueChanged?.Invoke(sender: this, e: coercedValue);
                PropertyChanged?.Invoke(sender: this, e: new(propertyName: nameof(Value)));
            }
        }
    }

    public event EventHandler<T> ValueChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    private T Coerce(T value)
    {
        return coercer is null ? value : coercer(value);
    }

    private bool SetValue(T oldValue, T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            SetValue(newValue);
            return true;
        }

        return false;
    }

    private void SetValue(T value)
    {
        LocalSettings.Values[key] = s_isEnum ? Convert.ToInt32(value) : value;
    }
}

file static class LocalSettings
{
    public static IPropertySet Values => s_store.Values;

    private static readonly ApplicationDataContainer s_store = ApplicationData.Current.LocalSettings;
}
