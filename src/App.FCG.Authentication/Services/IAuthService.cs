using FCG.Core.Web;

namespace FCG.Authentication.Services
{
	public interface IAuthService
	{
		string GenerateAuthToken(string email);
		IApiResponse<string> ApiResponseTest(string name);
	}
}
