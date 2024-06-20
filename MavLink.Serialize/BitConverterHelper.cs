using System.Runtime.InteropServices;
using System.Text;

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

  
    public static T[] ReadArray<T>(T[] array, ref ReadOnlySpan<byte> span) where T : struct
    {
        ArgumentNullException.ThrowIfNull(array);
        var size = Marshal.SizeOf<T>();
        var sizeToRead = array.Length * size;
        if (span.Length >= sizeToRead)
        {
            MemoryMarshal.Cast<byte, T>(span.Slice(0, sizeToRead)).CopyTo(array);
            span = span.Slice(sizeToRead);
        }
        else
        {
            var aligning = span.Length % size;
            if (aligning == 0)
            {
                MemoryMarshal.Cast<byte, T>(span).CopyTo(array);
                span = span.Slice(span.Length);                
            }
            else
            {
                var newLen = span.Length - aligning;
                MemoryMarshal.Cast<byte, T>(span.Slice(0, newLen)).CopyTo(array);
                span = span.Slice(newLen);
                array[newLen / size] = Read<T>(ref span);                
            }
        }
        return array;
    }

    public static string ReadString(int count, ref ReadOnlySpan<byte> span)
    {
        var result = Encoding.ASCII.GetString(span.Slice(0,count)).TrimEnd('\0');
        span = span.Slice(count);
        return result;
    }
    
    public static void WriteString(string value, int count, ref Span<byte> span)
    {
        Encoding.ASCII.GetBytes(value, span);
        span = span.Slice(count);
    }

    public static void WriteArray<T>(T[] array, ref Span<byte> span) where T : struct
    {
        ArgumentNullException.ThrowIfNull(array);
        
        var casted = MemoryMarshal.Cast<T, byte>(array);
        casted.CopyTo(span);
        span = span.Slice(casted.Length);
    }

    public static void Write<T>(in T value, ref Span<byte> span) where T : struct
    {
        var size = Marshal.SizeOf(default(T));
        MemoryMarshal.Write(span, value);
        span = span.Slice(size);
    }
}