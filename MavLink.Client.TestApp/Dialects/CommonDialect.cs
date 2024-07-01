using MavLink.Serialize.Dialects;

namespace MavLink.Client.TestApp.Dialects;

[Dialect("minimal.xml")]
public partial class MinimalDialect
{
}

[Dialect("standard.xml")]
[DialectDependency<MinimalDialect>]
public partial class StandardDialect
{
}

[Dialect("common.xml")]
[DialectDependency<StandardDialect>]
public partial class CommonDialect
{
}



[Dialect("ardupilotmega.xml")]
[DialectDependency<UAvionixDialect>]
[DialectDependency<IcarousDialect>]
[DialectDependency<CubepilotDialect>]
[DialectDependency<CsAirLinkDialect>]
[DialectDependency<CommonDialect>]
public partial class ArduPilotMegaDialect
{
}

[Dialect("uAvionix.xml")]
[DialectDependency<CommonDialect>]
public partial class UAvionixDialect
{
}

[Dialect("icarous.xml")]
public partial class IcarousDialect
{
}

[Dialect("cubepilot.xml")]
[DialectDependency<CommonDialect>]
public partial class CubepilotDialect
{
}

[Dialect("csAirLink.xml")]
public partial class CsAirLinkDialect
{
}