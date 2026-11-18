using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace Fluver.Extensions;

public static class RegistryExtensions
{
    extension(Registry)
    {
        public static bool TryGetValue<T>(string keyName, string valueName, [MaybeNullWhen(false)] out T value)
        {
            return TryGetValue(keyName, valueName, defaultValue: default, out value);
        }

        public static bool TryGetValue<T>(string keyName, string valueName, T defaultValue, [MaybeNullWhen(false)] out T value)
        {
            object obj;
            try
            {
                obj = Registry.GetValue(keyName, valueName, defaultValue: null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read Registry value \"{valueName}\" within key \"{keyName}\": {ex}");
                obj = null;
            }

            if (obj is T tValue)
            {
                value = tValue;
                return true;
            }

            value = defaultValue;
            return false;
        }
    }

    extension(RegistryKey key)
    {
        public bool TryOpenSubKey(string name, [NotNullWhen(true)] out RegistryKey subKey)
        {
            try
            {
                return (subKey = key.OpenSubKey(name)) is not null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open Registry subkey \"{name}\": {ex}");

                subKey = null;
                return false;
            }
        }

        public bool TryGetSubKeyNames([NotNullWhen(true)] out string[] names)
        {
            try
            {
                names = key.GetSubKeyNames();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to enumerate subkey names of Registry key \"{key.Name}\": {ex}");

                names = null;
                return false;
            }
        }

        public bool TryGetValue<T>(string name, [MaybeNullWhen(false)] out T value)
        {
            return key.TryGetValue(name, defaultValue: default, out value);
        }

        public bool TryGetValue<T>(string name, T defaultValue, [MaybeNullWhen(false)] out T value)
        {
            object obj;
            try
            {
                obj = key.GetValue(name, defaultValue: null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read Registry value \"{name}\" within key \"{key.Name}\": {ex}");
                obj = null;
            }

            if (obj is T tValue)
            {
                value = tValue;
                return true;
            }

            value = defaultValue;
            return false;
        }
    }
}
