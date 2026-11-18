using System.Runtime.CompilerServices;

namespace Fluver.Services.Settings;

public sealed partial class EnumSetting<T>(string key, T defaultValue = default, Func<T, T> coercer = null)
    : Setting<T>(key, defaultValue, coercer)
    where T : struct, Enum
{
    protected override object Box(T value)
    {
        return Unsafe.SizeOf<T>() switch
        {
            sizeof(sbyte) => Unsafe.As<T, sbyte>(ref value),
            sizeof(short) => Unsafe.As<T, short>(ref value),
            sizeof(int) => Unsafe.As<T, int>(ref value),
            sizeof(long) => Unsafe.As<T, long>(ref value),
            _ => Convert.ChangeType(value, typeof(T).GetEnumUnderlyingType())
        };
    }

    protected override T Unbox(object obj)
    {
        return Unsafe.SizeOf<T>() switch
        {
            sizeof(sbyte) when obj is sbyte b => Unsafe.As<sbyte, T>(ref b),
            sizeof(short) when obj is short s => Unsafe.As<short, T>(ref s),
            sizeof(int) when obj is int i => Unsafe.As<int, T>(ref i),
            sizeof(long) when obj is long l => Unsafe.As<long, T>(ref l),
            _ => (T)Convert.ChangeType(obj, typeof(T).GetEnumUnderlyingType())
        };
    }
}
