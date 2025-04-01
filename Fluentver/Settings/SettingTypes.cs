namespace Fluentver.Settings;

public class Setting<T>(string key, T defaultValue)
{
    static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

    public T Value
    {
        get
        {
            if (localSettings.Values.TryGetValue(key, out object value))
                return (T)value;
            else
            {
                localSettings.Values[key] = defaultValue;
                return defaultValue;
            }
        }
        set
        {
            localSettings.Values[key] = value;
            ValueChanged?.Invoke(this, value);
        }
    }

    public static implicit operator T(Setting<T> setting) => setting.Value;

    public event TypedEventHandler<Setting<T>, T> ValueChanged;
}

public class EnumSetting<T>(string key, T defaultValue) where T : Enum
{
    static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

    public T Value
    {
        get
        {
            if (localSettings.Values.TryGetValue(key, out object value))
                return (T)value;
            else
            {
                localSettings.Values[key] = (int)(object)defaultValue;
                return defaultValue;
            }
        }
        set
        {
            localSettings.Values[key] = (int)(object)value;
            ValueChanged?.Invoke(this, value);
        }
    }

    public static implicit operator T(EnumSetting<T> setting) => setting.Value;

    public event TypedEventHandler<EnumSetting<T>, T> ValueChanged;
}
