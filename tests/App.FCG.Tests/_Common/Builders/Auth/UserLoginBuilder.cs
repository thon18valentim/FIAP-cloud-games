using FCG.Shared.Dtos;

namespace FCG.Tests._Common.Builders.Auth;

/// <summary>
/// Builder for <see cref="UsuarioLogin"/> (UserLogin).
/// </summary>
public sealed class UserLoginBuilder
    : FCG.Tests._Common.Builders.BuilderBase<UserLoginBuilder, UsuarioLogin>
{
    private string _email = $"user_{Guid.NewGuid():N}@test.com";
    private string _password = "Abc!12345";

    public UserLoginBuilder WithEmail(string value) { _email = value; return this; }
    public UserLoginBuilder WithPassword(string value) { _password = value; return this; }

    public UserLoginBuilder Invalid_EmailFormat() { _email = "invalid"; return this; }
    public UserLoginBuilder Invalid_ShortPassword() { _password = "abc123"; return this; }

    protected override UsuarioLogin BuildInternal() => new UsuarioLogin
    {
        Email = _email,
        Senha = _password
    };
}
