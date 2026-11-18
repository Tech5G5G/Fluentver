using System.ComponentModel;
using Windows.Storage;
using Windows.Foundation.Collections;
using Fluver.Contracts;

namespace Fluver.Services.Settings;

public partial class Setting<T> : ISetting<T>
{
    public T Value
    {
        get => _value;
        set
        {
            var coercedValue = Coerce(value);

            if (!EqualityComparer<T>.Default.Equals(_value, coercedValue))
            {
                SetValue(_value = coercedValue);
                ValueChanged?.Invoke(sender: this, e: coercedValue);
                PropertyChanged?.Invoke(sender: this, e: new(propertyName: nameof(Value)));
            }
        }
    }

    public event EventHandler<T> ValueChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly string _key;
    private readonly Func<T, T> _coercer;

    private T _value;

    public Setting(string key, T defaultValue = default, Func<T, T> coercer = null)
    {
        _key = key;
        _coercer = coercer;

        var exists = LocalSettings.Values.TryGetValue(key, out object obj);

        var value = exists ? Unbox(obj) : defaultValue;
        var coercedValue = Coerce(value);

        if (!exists || !EqualityComparer<T>.Default.Equals(value, coercedValue))
        {
            SetValue(coercedValue);
        }

        _value = coercedValue;
    }

    private T Coerce(T value)
    {
        return _coercer is null ? value : _coercer(value);
    }

    private void SetValue(T value)
    {
        LocalSettings.Values[_key] = Box(value);
    }

    protected virtual object Box(T value)
    {
        return value;
    }

    protected virtual T Unbox(object obj)
    {
        return (T)obj;
    }
}

file static class LocalSettings
{
    public static IPropertySet Values => s_store.Values;

    private static readonly ApplicationDataContainer s_store = ApplicationData.Current.LocalSettings;
}
