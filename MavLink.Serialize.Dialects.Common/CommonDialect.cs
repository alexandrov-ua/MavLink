using MavLink.Serialize.Dialects;

namespace MavLink.Serialize.Dialects.Common;

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