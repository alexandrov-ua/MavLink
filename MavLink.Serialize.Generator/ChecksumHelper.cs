using System;

namespace MavLink.Serialize.Generator;

public static class ChecksumHelper
{
    const int X25InitChecksum = 0xffff;

    public static ushort Accumulate(byte b, ushort crc)
    {
        unchecked
        {
            byte ch = (byte)(b ^ (byte)(crc & 0x00ff));
            ch = (byte)(ch ^ (ch << 4));
            return (ushort)((crc >> 8) ^ (ch << 8) ^ (ch << 3) ^ (ch >> 4));
        }
    }

    public static ushort Accumulate(string buffer, ushort crc)
    {
        if (buffer.Length < 1)
        {
            return 0xffff;
        }
        ushort crcTmp = crc;

        for (int i = 0; i < buffer.Length; i++)
        {
            crcTmp = Accumulate((byte)buffer[i], crcTmp);
        }
       
        return (crcTmp);
    }
}