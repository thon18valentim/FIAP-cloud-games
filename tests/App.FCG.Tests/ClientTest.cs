using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using App.FCG.WebApi.Controllers.v1;
using FCG.Clients.Services;
using FCG.Core.Configurations.Identidade;
using FCG.Shared.Dtos;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace FCG.Tests;

public class AuthControllerRegistrarTests
{
    // ---------- Helpers para criar mocks de Identity ----------
    private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );
    }

    private static Mock<SignInManager<IdentityUser>> CreateSignInManagerMock(UserManager<IdentityUser> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        return new Mock<SignInManager<IdentityUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            null, null, null, null
        );
    }

    private static IOptions<AppSettings> CreateAppSettings()
    {
        return Options.Create(new AppSettings
        {
            Secret = "um-segredo-super-seguro-de-teste-32+chars",
            Emissor = "fcg-tests",
            ValidoEm = "fcg-tests",
            ExpiracaoHoras = 1
        });
    }

    private static AuthController CreateController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IClientService clientService,
        IOptions<AppSettings> appSettings)
    {
        var controller = new AuthController(signInManager, userManager, appSettings, clientService);

        // Se o MainController usa ModelState/HttpContext, garanta um ControllerContext básico:
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    private static UsuarioRegistro BuildUsuarioRegistro(string email = "user@test.com")
    {
        return new UsuarioRegistro
        {
            Email = email,
            Senha = "SenhaForte123!",
            SenhaConfirmacao = "SenhaForte123!",
            Nome = "User Teste",
            Cpf = "12345678900"
        };
    }

    // ---------- CENÁRIO 1: SUCESSO ----------
    [Fact]
    public async Task Registrar_DeveRetornarOk_ComToken_QuandoUsuarioEClienteCriados()
    {
        // Arrange
        var um = CreateUserManagerMock();
        var sm = CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();
        var appSettings = CreateAppSettings();

        var usuarioRegistro = BuildUsuarioRegistro();

        // Identity: CreateAsync OK e atribui um Id (string)
        um.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), usuarioRegistro.Senha))
          .ReturnsAsync(IdentityResult.Success)
          .Callback<IdentityUser, string>((user, _) => user.Id = Guid.NewGuid().ToString());

        // Identity: FindByEmailAsync retorna o mesmo user (com Id setado)
        um.Setup(x => x.FindByEmailAsync(usuarioRegistro.Email))
          .ReturnsAsync((string email) => new IdentityUser
          {
              Id = Guid.NewGuid().ToString(),
              Email = email,
              UserName = email,
              EmailConfirmed = true
          });

        // Identity: Claims e Roles vazias para simplificar
        um.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
          .ReturnsAsync(new List<Claim>());
        um.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
          .ReturnsAsync(new List<string>());

        // ClientService: Insert válido
        clientService.Setup(x => x.Insert(It.IsAny<ClienteRegistro>()))
                     .ReturnsAsync(new ValidationResult());

        var controller = CreateController(um.Object, sm.Object, clientService.Object, appSettings);

        // Act
        var result = await controller.Registrar(usuarioRegistro);

        // Assert
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();

        // O objeto deve conter AccessToken e ExpiresIn conforme seu UsuarioRespostaLogin
        dynamic payload = ok!.Value!;
        string? token = payload?.AccessToken;
        double expires = payload?.ExpiresIn;

        token.Should().NotBeNullOrEmpty();
        expires.Should().BeGreaterThan(0);
    }

    // ---------- CENÁRIO 2: CLIENTE INVÁLIDO -> ROLLBACK (DeleteAsync) + ERRO ----------
    [Fact]
    public async Task Registrar_DeveExcluirUsuarioERetornarErro_QuandoClienteInvalido()
    {
        // Arrange
        var um = CreateUserManagerMock();
        var sm = CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();
        var appSettings = CreateAppSettings();

        var usuarioRegistro = BuildUsuarioRegistro();

        // CreateAsync OK com Id
        var createdUser = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = usuarioRegistro.Email, UserName = usuarioRegistro.Email, EmailConfirmed = true };
        um.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), usuarioRegistro.Senha))
          .ReturnsAsync(IdentityResult.Success)
          .Callback<IdentityUser, string>((user, _) => user.Id = createdUser.Id);

        um.Setup(x => x.FindByEmailAsync(usuarioRegistro.Email))
          .ReturnsAsync(createdUser);

        // ClientService retorna ValidationResult com erro
        var invalidResult = new ValidationResult(new[] { new ValidationFailure("Cliente", "Falha ao registrar cliente") });
        clientService.Setup(x => x.Insert(It.IsAny<ClienteRegistro>()))
                     .ReturnsAsync(invalidResult);

        // DeleteAsync deve ser chamado
        um.Setup(x => x.DeleteAsync(It.Is<IdentityUser>(u => u.Id == createdUser.Id)))
          .ReturnsAsync(IdentityResult.Success)
          .Verifiable();

        var controller = CreateController(um.Object, sm.Object, clientService.Object, appSettings);

        // Act
        var result = await controller.Registrar(usuarioRegistro);

        // Assert
        var bad = result as BadRequestObjectResult;
        bad.Should().NotBeNull();

        // Verifica que o rollback (DeleteAsync) ocorreu
        um.Verify(x => x.DeleteAsync(It.Is<IdentityUser>(u => u.Id == createdUser.Id)), Times.Once);
    }

    // ---------- CENÁRIO 3: FALHA NO IDENTITY (CreateAsync) ----------
    [Fact]
    public async Task Registrar_DeveRetornarErro_QuandoCreateAsyncFalhar()
    {
        // Arrange
        var um = CreateUserManagerMock();
        var sm = CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();
        var appSettings = CreateAppSettings();

        var usuarioRegistro = BuildUsuarioRegistro();

        var identityError = IdentityResult.Failed(new IdentityError { Description = "Senha fraca" });
        um.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), usuarioRegistro.Senha))
          .ReturnsAsync(identityError);

        var controller = CreateController(um.Object, sm.Object, clientService.Object, appSettings);

        // Act
        var result = await controller.Registrar(usuarioRegistro);

        // Assert
        var bad = result as BadRequestObjectResult;
        bad.Should().NotBeNull();
    }

    // ---------- CENÁRIO 4: MODELSTATE INVÁLIDO ----------
    [Fact]
    public async Task Registrar_DeveRetornarErro_QuandoModelStateInvalido()
    {
        // Arrange
        var um = CreateUserManagerMock();
        var sm = CreateSignInManagerMock(um.Object);
        var clientService = new Mock<IClientService>();
        var appSettings = CreateAppSettings();

        var controller = CreateController(um.Object, sm.Object, clientService.Object, appSettings);
        controller.ModelState.AddModelError("Email", "Obrigatório");

        // Act
        var result = await controller.Registrar(new UsuarioRegistro());

        // Assert
        var bad = result as BadRequestObjectResult;
        bad.Should().NotBeNull();
    }
}
