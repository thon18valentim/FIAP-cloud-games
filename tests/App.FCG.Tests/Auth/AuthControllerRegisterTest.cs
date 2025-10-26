using System.Security.Claims;
using App.FCG.WebApi.Controllers.v1; // TODO: ajuste para o namespace do seu AuthController
using FCG.Clients.Services;          // TODO
using FCG.Core.Configurations.Identidade;
using FCG.Shared.Dtos;
using FCG.Tests._Common.Testing;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Tests.Auth;

public class AuthControllerRegisterTest
{
    private static IOptions<AppSettings> CreateAppSettings() =>
        Options.Create(new AppSettings
        {
            Secret = "super-secret-key-32chars-minimum-ABC123!",
            Emissor = "FCG",
            ValidoEm = "FCG",
            ExpiracaoHoras = 1
        });

    private static AuthController CreateController(
        SignInManager<IdentityUser> sm,
        UserManager<IdentityUser> um,
        IClientService clientService,
        IOptions<AppSettings> app) => new(sm, um, app, clientService);

    [Fact(DisplayName = "Register: should return 200 OK and JWT when data is valid")]
    public async Task Register_Should_ReturnOk_WithToken_When_Valid()
    {
        // Arrange
        var um = IdentityTestFactory.CreateUserManagerMock();
        var sm = IdentityTestFactory.CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();

        var userDto = new UsuarioRegistro
        {
            Nome = "John Doe",
            Cpf = "12345678909",
            Email = $"john_{Guid.NewGuid():N}@mail.com",
            Senha = "Abc!12345",
            SenhaConfirmacao = "Abc!12345"
        };

        // Identity user creation succeeds
        um.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), userDto.Senha))
          .ReturnsAsync(IdentityResult.Success)
          .Callback<IdentityUser, string>((user, _) => user.Id = Guid.NewGuid().ToString());

        // Find user + claims/roles
        um.Setup(x => x.FindByEmailAsync(userDto.Email))
          .ReturnsAsync(new IdentityUser { Id = Guid.NewGuid().ToString(), Email = userDto.Email, UserName = userDto.Email });
        um.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new List<Claim>());
        um.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new List<string> { "Customer" });

        // ClientService insert ok
        clientService.Setup(s => s.Insert(It.IsAny<ClienteRegistro>()))
                     .ReturnsAsync(new ValidationResult());

        var controller = CreateController(sm.Object, um.Object, clientService.Object, CreateAppSettings());

        // Act
        var result = await controller.Registrar(userDto);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        var payload = ok!.Value as UsuarioRespostaLogin;
        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        payload.UsuarioToken.Email.Should().Be(userDto.Email);

        clientService.Verify(s => s.Insert(It.Is<ClienteRegistro>(c => c.Email == userDto.Email)), Times.Once);
    }

    [Fact(DisplayName = "Register: should rollback user when client insertion throws")]
    public async Task Register_Should_RollbackUser_When_ClientInsertThrows()
    {
        var um = IdentityTestFactory.CreateUserManagerMock();
        var sm = IdentityTestFactory.CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();

        var userDto = new UsuarioRegistro
        {
            Nome = "John Doe",
            Cpf = "12345678909",
            Email = $"john_{Guid.NewGuid():N}@mail.com",
            Senha = "Abc!12345",
            SenhaConfirmacao = "Abc!12345"
        };

        um.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), userDto.Senha))
          .ReturnsAsync(IdentityResult.Success)
          .Callback<IdentityUser, string>((user, _) => user.Id = Guid.NewGuid().ToString());

        um.Setup(x => x.FindByEmailAsync(userDto.Email))
          .ReturnsAsync(new IdentityUser { Id = Guid.NewGuid().ToString(), Email = userDto.Email });

        clientService.Setup(s => s.Insert(It.IsAny<ClienteRegistro>()))
                     .ThrowsAsync(new InvalidOperationException("DB failed"));

        var controller = CreateController(sm.Object, um.Object, clientService.Object, CreateAppSettings());

        // Act + Assert
        await FluentActions.Awaiting(() => controller.Registrar(userDto))
            .Should().ThrowAsync<InvalidOperationException>();

        um.Verify(x => x.DeleteAsync(It.IsAny<IdentityUser>()), Times.Once);
    }

    public static IEnumerable<object[]> MissingFieldCases()
    {
        // (fieldName, UsuarioRegistro with field missing/invalid)
        yield return new object[] { "Email", new UsuarioRegistro { Nome = "A", Cpf = "12345678909", Email = "", Senha = "Abc!12345", SenhaConfirmacao = "Abc!12345" } };
        yield return new object[] { "Cpf", new UsuarioRegistro { Nome = "A", Cpf = "", Email = "a@test.com", Senha = "Abc!12345", SenhaConfirmacao = "Abc!12345" } };
        yield return new object[] { "Nome", new UsuarioRegistro { Nome = "", Cpf = "12345678909", Email = "a@test.com", Senha = "Abc!12345", SenhaConfirmacao = "Abc!12345" } };
        yield return new object[] { "Senha", new UsuarioRegistro { Nome = "A", Cpf = "12345678909", Email = "a@test.com", Senha = "", SenhaConfirmacao = "" } };
    }

    [Theory(DisplayName = "Register: should return 400 when data is missing")]
    [MemberData(nameof(MissingFieldCases))]
    public async Task Register_Should_ReturnBadRequest_When_MissingData(string field, UsuarioRegistro invalid)
    {
        var um = IdentityTestFactory.CreateUserManagerMock();
        var sm = IdentityTestFactory.CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();
        var controller = CreateController(sm.Object, um.Object, clientService.Object, CreateAppSettings());

        // Simula validação de DataAnnotations preenchendo o ModelState
        ModelStateTestHelper.ValidateAndPopulateModelState(controller, invalid);

        var result = await controller.Registrar(invalid);

        var bad = result as BadRequestObjectResult;
        bad.Should().NotBeNull($"missing/invalid {field} must yield 400");
        um.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        clientService.Verify(s => s.Insert(It.IsAny<ClienteRegistro>()), Times.Never);
    }
}
