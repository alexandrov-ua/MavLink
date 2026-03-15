using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using MavLink.Serialize.Dialects.Common;

namespace MavLink.Client.Transports;

public class SerialTransport : IMavLinkTransport
{
    private SerialPort _port;

    public SerialTransport(string deviceName, int baud)
    {
        _port = new SerialPort(deviceName, baud, Parity.None, 8, StopBits.One);
        _port.Handshake = Handshake.None;
        _port.ReadTimeout = 500;
        _port.WriteTimeout = 500;
        _port.Open();
    }

    public void Dispose()
    {
        _port.Dispose();
    }

    public int Receive(Span<byte> buffer)
    {
        while (true)
        {
            var first = _port.BaseStream.ReadByte();
            if (first == -1)
            {
                break;
            }
            if (first == 0xFD || first == 0xFE)
            {
                var len_byte = _port.BaseStream.ReadByte();
                var len = 10 + len_byte;
                var begin = new byte[]{ (byte)first, (byte)len_byte };
                begin.CopyTo(buffer.Slice(0,2));
                _port.BaseStream.ReadExactly(buffer.Slice(2, len));
                return len + 2;
            }
        }
        return 0;
    }

    public async Task Send(ReadOnlyMemory<byte> memory)
    {
        await _port.BaseStream.WriteAsync(memory);
        await _port.BaseStream.FlushAsync();
    }
}