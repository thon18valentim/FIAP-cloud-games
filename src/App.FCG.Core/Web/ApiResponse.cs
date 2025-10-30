using System.Net;

namespace FCG.Core.Web
{
	public class ApiResponse<TResult> : IApiResponse<TResult>
	{
		public TResult? ResultValue { get; set; }
		public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
		public string Message { get; set; } = string.Empty;
		public bool IsSuccess { get; set; }

		public ApiResponse(TResult? resultValue, HttpStatusCode statusCode, string message, bool isSuccess)
		{
			ResultValue = resultValue;
			StatusCode = statusCode;
			Message = message;
			IsSuccess = isSuccess;
		}

		public ApiResponse() { }
	}
}
