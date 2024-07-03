using MavLink.Serialize.Messages;

namespace MavLink.Serialize;

public interface ICommandProvider<T> where T : IPayload
{
    IPocket<T> Create(uint mavCmd, Func<int, float> fieldInitializer);
}