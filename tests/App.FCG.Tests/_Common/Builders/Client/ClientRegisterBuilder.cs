using FCG.Shared.Dtos;

namespace FCG.Tests._Common.Builders.Clients;

/// <summary>
/// Builder for <see cref="ClienteRegistro"/>.
/// Wraps creation logic for clients derived from a UserRegister (UsuarioRegistro).
/// </summary>
public sealed class ClientRegisterBuilder
    : FCG.Tests._Common.Builders.BuilderBase<ClientRegisterBuilder, ClienteRegistro>
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Client";
    private string _cpf = "98765432100";
    private string _email = $"client_{Guid.NewGuid():N}@mail.com";
    private UsuarioRegistro? _user;

    public ClientRegisterBuilder WithId(Guid id) { _id = id; return this; }
    public ClientRegisterBuilder WithName(string name) { _name = name; return this; }
    public ClientRegisterBuilder WithCpf(string cpf) { _cpf = cpf; return this; }
    public ClientRegisterBuilder WithEmail(string email) { _email = email; return this; }

    /// <summary>Use an existing UserRegister to derive the Client.</summary>
    public ClientRegisterBuilder FromUser(UsuarioRegistro user)
    {
        _user = user;
        _name = user.Nome;
        _cpf = user.Cpf;
        _email = user.Email;
        return this;
    }

    protected override ClienteRegistro BuildInternal()
    {
        var user = _user ?? new UsuarioRegistro
        {
            Nome = _name,
            Cpf = _cpf,
            Email = _email,
            Senha = "Abc!12345",
            SenhaConfirmacao = "Abc!12345"
        };

        return new ClienteRegistro(_id, user);
    }
}
