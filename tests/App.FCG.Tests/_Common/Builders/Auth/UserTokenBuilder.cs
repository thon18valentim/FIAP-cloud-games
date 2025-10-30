using FCG.Shared.Dtos;

namespace FCG.Tests._Common.Builders.Auth;

/// <summary>
/// Builder for <see cref="UsuarioToken"/>.
/// </summary>
public sealed class UserTokenBuilder
    : FCG.Tests._Common.Builders.BuilderBase<UserTokenBuilder, UsuarioToken>
{
    private string _id = Guid.NewGuid().ToString("N");
    private string _email = $"user_{Guid.NewGuid():N}@test.com";
    private readonly List<UsuarioClaim> _claims = new()
    {
        new UsuarioClaim { Type = "role", Value = "User" }
    };

    public UserTokenBuilder WithId(string id) { _id = id; return this; }
    public UserTokenBuilder WithEmail(string email) { _email = email; return this; }
    public UserTokenBuilder ClearClaims() { _claims.Clear(); return this; }

    public UserTokenBuilder AddClaim(string type, string value)
    {
        _claims.Add(new UsuarioClaim { Type = type, Value = value });
        return this;
    }

    public UserTokenBuilder AsAdmin()
    {
        _claims.Add(new UsuarioClaim { Type = "role", Value = "Admin" });
        return this;
    }

    protected override UsuarioToken BuildInternal() => new UsuarioToken
    {
        Id = _id,
        Email = _email,
        Claims = _claims.ToArray()
    };
}
