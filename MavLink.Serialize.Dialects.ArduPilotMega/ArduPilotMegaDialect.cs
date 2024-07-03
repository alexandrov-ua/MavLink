using MavLink.Serialize.Dialects.Common;

namespace MavLink.Serialize.Dialects.ArduPilotMega;

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