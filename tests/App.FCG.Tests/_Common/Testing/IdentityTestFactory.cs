using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Tests._Common.Testing;

public static class IdentityTestFactory
{
    public static Mock<UserManager<IdentityUser>> CreateUserManagerMock(IdentityOptions? opts = null)
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object,
            Options.Create(opts ?? new IdentityOptions { User = { RequireUniqueEmail = true } }),
            new PasswordHasher<IdentityUser>(),
            // IUserValidator
            new IUserValidator<IdentityUser>[] { new UserValidator<IdentityUser>() },
            // IPasswordValidator
            new IPasswordValidator<IdentityUser>[] { new PasswordValidator<IdentityUser>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            null
        );
    }

    public static Mock<SignInManager<IdentityUser>> CreateSignInManagerMock(UserManager<IdentityUser> um)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        return new Mock<SignInManager<IdentityUser>>(
            um, contextAccessor.Object, claimsFactory.Object, null, null, null, null
        );
    }
}
