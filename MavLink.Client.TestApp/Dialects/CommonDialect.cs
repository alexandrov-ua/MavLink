using MavLink.Serialize.Dialects;

namespace MavLink.Client.TestApp.Dialects;

[Dialect("minimal.xml")]
public partial class MinimalDialect
{
}

[Dialect("standard.xml")]
public partial class StandardDialect
{
}

[Dialect("common.xml")]
public partial class CommonDialect
{
    public static readonly IDialect Default = CommonDialect.Create(StandardDialect.Create(MinimalDialect.Create()));
}



[Dialect("ardupilotmega.xml")]
public partial class ArduPilotMegaDialect
{
    public static readonly IDialect Default =
        ArduPilotMegaDialect.Create(CommonDialect.Default,
            UAvionixDialect.Create(CommonDialect.Default),
            IcarousDialect.Create(), 
            CubepilotDialect.Create(CommonDialect.Default), 
            CsAirLinkDialect.Create());
}

[Dialect("uAvionix.xml")]
public partial class UAvionixDialect
{
}

[Dialect("icarous.xml")]
public partial class IcarousDialect
{
}

[Dialect("cubepilot.xml")]
public partial class CubepilotDialect
{
}

[Dialect("csAirLink.xml")]
public partial class CsAirLinkDialect
{
}