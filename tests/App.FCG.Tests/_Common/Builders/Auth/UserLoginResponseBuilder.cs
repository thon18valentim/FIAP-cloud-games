using FCG.Shared.Dtos;

namespace FCG.Tests._Common.Builders.Auth;

/// <summary>
/// Builder for <see cref="UsuarioRespostaLogin"/>.
/// </summary>
public sealed class UserLoginResponseBuilder
    : FCG.Tests._Common.Builders.BuilderBase<UserLoginResponseBuilder, UsuarioRespostaLogin>
{
    private string _accessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    private double _expiresIn = TimeSpan.FromHours(1).TotalSeconds;
    private UsuarioToken _userToken = new UserTokenBuilder().Build();

    public UserLoginResponseBuilder WithToken(string token) { _accessToken = token; return this; }
    public UserLoginResponseBuilder ExpiresIn(TimeSpan span) { _expiresIn = span.TotalSeconds; return this; }
    public UserLoginResponseBuilder WithUserToken(UsuarioToken token) { _userToken = token; return this; }

    protected override UsuarioRespostaLogin BuildInternal() => new UsuarioRespostaLogin
    {
        AccessToken = _accessToken,
        ExpiresIn = _expiresIn,
        UsuarioToken = _userToken
    };
}
