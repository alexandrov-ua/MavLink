namespace MavLink.Serialize;

public static class ChecksumHelper
{
    const int X25InitChecksum = 0xffff;

    private static ushort Accumulate(byte b, ushort crc)
    {
        unchecked
        {
            byte ch = (byte)(b ^ (byte)(crc & 0x00ff));
            ch = (byte)(ch ^ (ch << 4));
            return (ushort)((crc >> 8) ^ (ch << 8) ^ (ch << 3) ^ (ch >> 4));
        }
    }

    public static ushort Calculate(ReadOnlySpan<byte> buffer, byte crcExtra)
    {
        if (buffer.Length < 1)
        {
            return 0xffff;
        }
        ushort crcTmp = X25InitChecksum;

        for (int i = 1; i < buffer.Length; i++)
        {
            crcTmp = Accumulate(buffer[i], crcTmp);
        }
        
        crcTmp = Accumulate(crcExtra, crcTmp); 
        return (crcTmp);
    }

}