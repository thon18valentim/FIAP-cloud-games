
namespace FCG.Authentication.Services
{
	public interface IAuthService
	{
		string GenerateAuthToken(string email);
	}
}
