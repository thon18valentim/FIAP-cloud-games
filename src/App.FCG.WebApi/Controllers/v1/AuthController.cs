using FCG.Authentication.Services;
using FCG.Clients.Services;
using FCG.Core.Web;
using FCG.Shared.Dtos;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.FCG.WebApi.Controllers.v1;

[Route("api/identidade")]
public class AuthController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager; 
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IClientService _clientService;
    private readonly IAuthService _authService;

    public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IClientService clientService,
                      IAuthService authService, IUser user,INotificador notificador) : base(notificador, user)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _clientService = clientService;
        _authService = authService;
    }

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

        var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

        if (result.Succeeded)
        {
            var clienteResult = await RegistrarCliente(usuarioRegistro);

            if (!clienteResult.IsValid)
            {
                await _userManager.DeleteAsync(user);
                return CustomResponse(clienteResult);
            }

            return CustomResponse(_authService.GenerateAuthToken(usuarioRegistro.Email));
        }

        foreach (var erro in result.Errors)
        {
            NotificarErro(erro.Description);
        }

        return CustomResponse();
    }

    [HttpPost("auth")]
    public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

        if (result.Succeeded)
        {
            return CustomResponse(_authService.GenerateAuthToken(usuarioLogin.Email));
        }

        if (result.IsLockedOut)
        {
            NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
            return CustomResponse();
        }

        NotificarErro("Usuário ou senha incorretos");
        return CustomResponse();
    }

    [HttpGet("test/{name}")]
    public ActionResult Test(string name)
    {
        var result = _authService.ApiResponseTest(name);
        return StatusCode(result.StatusCode.GetHashCode(), result);
    }

    private async Task<ValidationResult> RegistrarCliente(UsuarioRegistro usuarioRegistro)
    {
        var usuario = await _userManager.FindByEmailAsync(usuarioRegistro.Email);

        var cliente = new ClienteRegistro(Guid.Parse(usuario.Id), usuarioRegistro);

        try
        {
            return await _clientService.Insert(cliente);
        }
        catch
        {
            await _userManager.DeleteAsync(usuario);
            throw;
        }
    }
}
