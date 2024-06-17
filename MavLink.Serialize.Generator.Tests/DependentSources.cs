namespace MavLink.Serialize.Generator.Tests;

public static class DependentSources
{
    public static List<string> List = new List<string>()
    {
        """
        namespace MavLink.Serialize.Messages;
        public interface IPocket<out TPayload> where TPayload : IPayload
        {
        
        }
        
        public interface IPayload
        {
        
        }
        """,
        """
        using System;
        using MavLink.Serialize.Messages;
        namespace MavLink.Serialize.Dialects
        {
            public interface IDialect
            {
                IPocket<IPayload> CreatePocket(uint messageId, bool isMavlinkV2, byte sequenceNumber,
                    byte systemId, byte componentId, ReadOnlySpan<byte> payload);
            }
        }
        """
    };
}