using System.Collections.Generic;
using System.Threading.Tasks;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Providers;

namespace MavLink.Client.DemoApp.Controls;

internal sealed class VehiclePositionProvider : MemoryProvider, IDynamic
{
    public event DataChangedEventHandler? DataChanged;

    private VehiclePosition _coordinates = new VehiclePosition(0, 0, 0, 0);

    public void SetVal(VehiclePosition position)
    {
        _coordinates = position;
        OnDataChanged();
    }

    void IDynamic.DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new DataChangedEventArgs());
    }

    public override Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        var vehicleFeature = new VehicleFeature(_coordinates, "Vehicle1");
        return Task.FromResult((IEnumerable<IFeature>) [vehicleFeature]);
    }
}

public class VehicleFeature : PointFeature
{
    public double Rotation
    {
        get => (double)this["rotation"]!;
        set => this["rotation"] = value;
    }

    public string Id
    {
        get => (string)this["ID"]!;
        set => this["ID"] = value;
    }

    public VehicleFeature(VehiclePosition position, string id) 
        : base(SphericalMercator.FromLonLat(position.Longitude, position.Latitude).ToMPoint())
    {
        Rotation = position.Rotation;
        Id = id;
    }

    public VehicleFeature(VehicleFeature pointFeature) : base(pointFeature)
    {
        Rotation = pointFeature.Rotation;
        Id = pointFeature.Id;
    }
}