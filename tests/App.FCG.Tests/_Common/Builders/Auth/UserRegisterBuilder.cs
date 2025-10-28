using FCG.Shared.Dtos;

namespace FCG.Tests._Common.Builders.Auth;

/// <summary>
/// Builder for <see cref="UserRegister"/>.
/// Generates valid defaults by design.
/// </summary>
public sealed class UserRegisterBuilder
    : FCG.Tests._Common.Builders.BuilderBase<UserRegisterBuilder, UsuarioRegistro>
{
    private string _name = "Test User";
    private string _cpf = "12345678909"; // Replace with a valid test CPF if needed
    private string _email = $"user_{Guid.NewGuid():N}@test.com";
    private string _password = "Abc!12345";
    private string _confirmPassword = "Abc!12345";

    public UserRegisterBuilder WithName(string value) { _name = value; return this; }
    public UserRegisterBuilder WithCpf(string value) { _cpf = value; return this; }
    public UserRegisterBuilder WithEmail(string value) { _email = value; return this; }
    public UserRegisterBuilder WithPassword(string value)
    {
        _password = value; _confirmPassword = value; return this;
    }
    public UserRegisterBuilder WithPassword(string value, string confirm)
    {
        _password = value; _confirmPassword = confirm; return this;
    }

    // Invalid scenarios for test coverage
    public UserRegisterBuilder Invalid_ShortPassword()
    { _password = "Abc123"; _confirmPassword = _password; return this; }

    public UserRegisterBuilder Invalid_PasswordMismatch()
    { _password = "Abc!12345"; _confirmPassword = "Abc!1234X"; return this; }

    public UserRegisterBuilder Invalid_EmailFormat()
    { _email = "invalid-email"; return this; }

    protected override UsuarioRegistro BuildInternal() => new UsuarioRegistro
    {
        Nome = _name,
        Cpf = _cpf,
        Email = _email,
        Senha = _password,
        SenhaConfirmacao = _confirmPassword
    };
}
