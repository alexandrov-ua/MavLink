using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MavLink.Client.Transports;
using MavLink.Serialize.Dialects;
using MavLink.Serialize.Dialects.Common;

namespace MavLink.Client.CommonVehicle;

public interface IVehicleMultiplexer : IDisposable
{
    IObservable<IEnumerable<VehicleInfo>> AvailableVehicles { get; }
    List<IVehicle> ConnectedVehicles { get; }
    IVehicle? CurrentVehicle { get; }
    public bool IsConnected { get; }
    public string? ConnectionString { get; }

    void Connect(string connectionString, IDialect? dialect = null);
    IVehicle ConnectToVehicle(VehicleInfo vehicleInfo);
    void DisconnectFromVehicle(IVehicle vehicle);
    void Disconnect();
}

public class VehicleMultiplexer : IVehicleMultiplexer
{
    private IMavLinkReactiveClient? _client;

    private BehaviorSubject<IEnumerable<VehicleInfo>> _availableVehicles =
        new BehaviorSubject<IEnumerable<VehicleInfo>>([]);

    public IObservable<IEnumerable<VehicleInfo>> AvailableVehicles => _availableVehicles;
    private IDisposable? _eventsSubscription = null;
    public List<IVehicle> ConnectedVehicles { get; } = new List<IVehicle>();
    public string? ConnectionString { get; private set; }

    public IVehicle? CurrentVehicle => ConnectedVehicles.FirstOrDefault();
    public bool IsConnected => _client != null;

    public void Connect(string connectionString, IDialect? dialect = null)
    {
        if (dialect == null)
        {
            dialect = CommonDialect.Default;
        }

        if (IsConnected)
        {
            throw new InvalidOperationException();
        }

        ConnectionString = connectionString;
        _client = new MavLinkReactiveClient(MavLinkClient.Create(connectionString, dialect));
        _eventsSubscription = _client.OfType<HeartbeatPocket>()
            .Buffer(TimeSpan.FromSeconds(2))
            .Select(t => t.GroupBy(q => (q.SystemId, q.ComponentId))
                .Select(g => g.First())
                .Select(p => new VehicleInfo(p.SystemId, p.ComponentId, p.Payload.Type.ToString(),
                    p.Payload.Autopilot.ToString()))
                .ToList())
            .Subscribe(_availableVehicles);
    }

    public void Disconnect()
    {
        if (IsConnected)
        {
            _client?.Dispose();
            _eventsSubscription?.Dispose();
            _client = null;
            _availableVehicles.Dispose();
            _availableVehicles = new BehaviorSubject<IEnumerable<VehicleInfo>>(new List<VehicleInfo>());
            foreach (var v in ConnectedVehicles)
            {
                v.Dispose();
            }
            ConnectedVehicles.Clear();
            ConnectionString = null;
        }
    }

    public IVehicle ConnectToVehicle(byte systemId, byte componentId)
    {
        if(!IsConnected)
            throw new InvalidOperationException("Connect to network first. Call .Connect(string)");
        
        var vehicle = new CommonVehicle(_client!, systemId, componentId);
        ConnectedVehicles.Insert(0, vehicle);
        return vehicle;
    }

    public IVehicle ConnectToVehicle(VehicleInfo vehicleInfo)
    {
        return ConnectToVehicle(vehicleInfo.SystemId, vehicleInfo.ComponentId);
    }
    
    public void DisconnectFromVehicle(IVehicle vehicle)
    {
        ConnectedVehicles.Remove(vehicle);
        vehicle.Dispose();
    }

    public void Dispose()
    {
        if (IsConnected)
        {
            _client?.Dispose();
            _eventsSubscription?.Dispose();            
        }
    }
}

public record VehicleInfo(byte SystemId, byte ComponentId, string VehicleType, string AutopilotName);