using Microsoft.AspNetCore.Identity;
using FCG.Authentication.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using FCG.Clients.Services;
using FCG.Shared.Dtos;

namespace App.FCG.WebApi.Controllers.v1;

[Route("api/identidade")]
public class AuthController(SignInManager<IdentityUser> signInManager,
					  UserManager<IdentityUser> userManager,
					  IClientService clientService,
                      IAuthService authService) : MainController
{
	[HttpPost("new-account")]
    public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = new IdentityUser
        {
            UserName = usuarioRegistro.Email,
            Email = usuarioRegistro.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, usuarioRegistro.Senha);

        if (result.Succeeded)
        {
            var clienteResult = await RegistrarCliente(usuarioRegistro);

            if (!clienteResult.IsValid)
            {
                await userManager.DeleteAsync(user);
                return CustomResponse(clienteResult);
            }

            return CustomResponse(authService.GenerateAuthToken(usuarioRegistro.Email));
        }

        foreach (var erro in result.Errors)
        {
            AdicionarErroProcessamento(erro.Description);
        }

        return CustomResponse();
    }

    [HttpPost("auth")]
    public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
    {
		if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

        if (result.Succeeded)
        {
            return CustomResponse(authService.GenerateAuthToken(usuarioLogin.Email));
        }

        if (result.IsLockedOut)
        {
            AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");
            return CustomResponse();
        }

        AdicionarErroProcessamento("Usuário ou senha incorretos");
        return CustomResponse();
    }

    [HttpGet("test/{name}")]
    public ActionResult Test(string name)
    {
        var result = authService.ApiResponseTest(name);
        return StatusCode(result.StatusCode.GetHashCode(), result);
    }

    private async Task<ValidationResult> RegistrarCliente(UsuarioRegistro usuarioRegistro)
    {
        var usuario = await userManager.FindByEmailAsync(usuarioRegistro.Email);

        var cliente = new ClienteRegistro(Guid.Parse(usuario.Id), usuarioRegistro);

        try
        {
            return await clientService.Insert(cliente);
        }
        catch
        {
            await userManager.DeleteAsync(usuario);
            throw;
        }
    }
}
