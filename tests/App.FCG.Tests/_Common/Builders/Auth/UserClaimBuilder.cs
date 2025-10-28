using FCG.Shared.Dtos;

namespace FCG.Tests._Common.Builders.Auth;

/// <summary>
/// Builder for <see cref="UsuarioClaim"/>.
/// </summary>
public sealed class UserClaimBuilder
    : FCG.Tests._Common.Builders.BuilderBase<UserClaimBuilder, UsuarioClaim>
{
    private string _type = "role";
    private string _value = "User";

    public UserClaimBuilder WithType(string type) { _type = type; return this; }
    public UserClaimBuilder WithValue(string value) { _value = value; return this; }

    protected override UsuarioClaim BuildInternal() => new UsuarioClaim
    {
        Type = _type,
        Value = _value
    };
}
