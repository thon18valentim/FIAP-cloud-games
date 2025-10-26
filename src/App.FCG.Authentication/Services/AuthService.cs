using FCG.Core.Services;
using FCG.Core.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace FCG.Authentication.Services
{
	public class AuthService(IConfiguration configuration) : BaseService, IAuthService
	{
		public string GenerateAuthToken(string email)
		{
			var secretKey = configuration["Jwt:Key"];
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity([
					new Claim(JwtRegisteredClaimNames.Email, email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()),
					new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
					]),
				Expires = DateTime.Now.AddMinutes(int.Parse(configuration["Jwt:Expiration"]!)),
				SigningCredentials = credentials,
				Issuer = configuration["Jwt:Issuer"],
				Audience = configuration["Jwt:Audience"]
			};

			var handler = new JsonWebTokenHandler();
			return handler.CreateToken(tokenDescriptor);
		}

		public IApiResponse<string> ApiResponseTest(string name)
		{
			if (name == "erro")
			{
				return Fail<string>("Some error here");
			}

			return Success<string>($"Hello world {name}");
		}

		#region :: private ::

		private static long ToUnixEpochDate(DateTime date)
			=> (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

		#endregion
	}
}
