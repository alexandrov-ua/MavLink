using System.Runtime.InteropServices;

namespace MavLink.Serialize;

public static class BitConverterHelper
{
    public static T Read<T>(ref ReadOnlySpan<byte> span) where T : struct
    {
        var size = Marshal.SizeOf(default(T));

        if (span.Length >= size)
        {
            var val = MemoryMarshal.Read<T>(span);
            span = span.Slice(size);
            return val;
        }

        if (span.Length < size && span.Length > 0)
        {
            Span<byte> tmp = stackalloc byte[size];
            span.CopyTo(tmp);
            span = span.Slice(span.Length);
            return MemoryMarshal.Read<T>(tmp);
        }

        return default(T);
    }

    public static void ReadArray<T>(T[] array, ref ReadOnlySpan<byte> span) where T : struct
    {
        ArgumentNullException.ThrowIfNull(array);//TODO: use span.CopyTo and MemoryMarshal.Cast<>
        
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Read<T>(ref span);
        }
    }
    
    public static void WriteArray<T>(T[] array, ref Span<byte> span) where T : struct
    {
        ArgumentNullException.ThrowIfNull(array);//TODO: use span.CopyTo and MemoryMarshal.Cast<>
        
        for (int i = 0; i < array.Length; i++)
        {
            Write<T>(array[i], ref span);
        }
    }

    public static void Write<T>(in T value, ref Span<byte> span) where T : struct
    {
        var size = Marshal.SizeOf(default(T));
        MemoryMarshal.Write(span, value);
        span = span.Slice(size);
    }
}