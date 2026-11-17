using SebastianGuzmanMorla.SmartEnum.Attributes;

namespace SebastianGuzmanMorla.SmartEnum.Tests.Types;

[GenerateSmartEnum]
public sealed partial class FormattedSecurityPermission : SmartEnum<FormattedSecurityPermission, string>
{
    public static readonly FormattedSecurityPermission Canonical = new("read", "read-alias");
    public static readonly FormattedSecurityPermission Legacy = new("legacy", "read");

    private readonly string _display;

    private FormattedSecurityPermission(string value, string display) : base(value)
    {
        _display = display;
    }

    public override string ToString() => _display;
}
