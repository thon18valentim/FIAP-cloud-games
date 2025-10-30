using FCG.Tests._Common.Fixtures;
using Microsoft.AspNetCore.Identity;

namespace FCG.Tests._Common.Base;

public abstract class IdentityTestBase
{
    protected readonly IdentityTestFixture Fixture;
    protected IdentityTestBase(IdentityTestFixture? f = null) => Fixture = f ?? new IdentityTestFixture();

    protected UserManager<IdentityUser> UserManager => Fixture.UserManagerMock.Object;
    protected SignInManager<IdentityUser> SignInManager => Fixture.SignInManagerMock.Object;
    protected PasswordOptions PasswordOptions => Fixture.PasswordOptions;
}
