using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Tests._Common.Fixtures;

public sealed class IdentityTestFixture
{
    public PasswordOptions PasswordOptions { get; }
    public Mock<UserManager<IdentityUser>> UserManagerMock { get; }
    public Mock<SignInManager<IdentityUser>> SignInManagerMock { get; }

    public IdentityTestFixture(PasswordOptions? options = null)
    {
        PasswordOptions = options ?? TestConfig.PasswordPolicy;

        // --- UserManager<TUser> ---
        var store = new Mock<IUserStore<IdentityUser>>();
        var identityOptions = Options.Create(new IdentityOptions { Password = PasswordOptions });

        var userValidators = Array.Empty<IUserValidator<IdentityUser>>();
        var passwordValidators = new IPasswordValidator<IdentityUser>[] { new PasswordValidator<IdentityUser>() };

        UserManagerMock = new Mock<UserManager<IdentityUser>>(
            store.Object,
            identityOptions,
            new PasswordHasher<IdentityUser>(),
            userValidators,
            passwordValidators,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<IdentityUser>>>()
        );

        // --- SignInManager<TUser> ---
        var http = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        var signInOptions = Options.Create(new IdentityOptions()); // pode reaproveitar "identityOptions" se preferir
        var signInLogger = Mock.Of<ILogger<SignInManager<IdentityUser>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<IdentityUser>>();

        SignInManagerMock = new Mock<SignInManager<IdentityUser>>(
            UserManagerMock.Object,
            http.Object,
            claimsFactory.Object,
            signInOptions,
            signInLogger,
            schemes.Object,
            confirmation.Object
        );
    }
}
