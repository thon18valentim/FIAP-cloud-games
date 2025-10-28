using System.Security.Claims;
using App.FCG.WebApi.Controllers.v1; // TODO
using FCG.Clients.Services;          // TODO
using FCG.Core.Configurations.Identidade;
using FCG.Shared.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Tests.Auth;

public class AuthController_Login_Tests
{
    private static IOptions<AppSettings> App() =>
        Options.Create(new AppSettings
        {
            Secret = "super-secret-key-32chars-minimum-ABC123!",
            Emissor = "FCG",
            ValidoEm = "FCG",
            ExpiracaoHoras = 1
        });

    [Fact(DisplayName = "Login: should return 200 and JWT when credentials are valid")]
    public async Task Login_Should_ReturnOk_WithToken_When_Valid()
    {
        var um = _Common.Testing.IdentityTestFactory.CreateUserManagerMock();
        var sm = _Common.Testing.IdentityTestFactory.CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();

        var login = new UsuarioLogin { Email = "valid@test.com", Senha = "Abc!12345" };

        // sign-in ok
        sm.Setup(x => x.PasswordSignInAsync(login.Email, login.Senha, false, true))
          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        um.Setup(x => x.FindByEmailAsync(login.Email))
          .ReturnsAsync(new IdentityUser { Id = Guid.NewGuid().ToString(), Email = login.Email });

        um.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
          .ReturnsAsync(new List<Claim> { new(ClaimTypes.Email, login.Email) });

        um.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
          .ReturnsAsync(new List<string> { "Customer" });

        var controller = new AuthController(sm.Object, um.Object, App(), clientService.Object);

        var result = await controller.Login(login);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        var payload = ok!.Value as UsuarioRespostaLogin;
        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        payload.UsuarioToken.Email.Should().Be(login.Email);
        payload.UsuarioToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email);
    }

    [Fact(DisplayName = "Login: should return 400 when credentials are invalid")]
    public async Task Login_Should_ReturnBadRequest_When_Invalid()
    {
        var um = _Common.Testing.IdentityTestFactory.CreateUserManagerMock();
        var sm = _Common.Testing.IdentityTestFactory.CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();

        var login = new UsuarioLogin { Email = "invalid@test.com", Senha = "Wrong!123" };

        sm.Setup(x => x.PasswordSignInAsync(login.Email, login.Senha, false, true))
          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var controller = new AuthController(sm.Object, um.Object, App(), clientService.Object);

        var result = await controller.Login(login);

        (result as BadRequestObjectResult).Should().NotBeNull();
    }
}
