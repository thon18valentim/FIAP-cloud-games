using FCG.Core.Web;
using System.Net;

namespace FCG.Core.Services
{
	public abstract class BaseService
	{
		/// <summary>
		/// Returns success for the request, with an object as result value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resultValue"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> Success<T>(T resultValue, string message = "")
		{
			return RequestSuccess(value: resultValue, isSuccess: true, message: message);
		}

		/// <summary>
		/// Returns success for the request, with an empty result value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> Success<T>(string message = "")
		{
			return RequestSuccess(value: default(T), isSuccess: true, message: message);
		}

		/// <summary>
		/// Returns success for the request, with a boolean result value
		/// </summary>
		/// <param name="resultValue"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<bool> Success(bool resultValue = true, string message = "")
		{
			return RequestSuccess(value: resultValue, isSuccess: true, message: message);
		}

		/// <summary>
		/// Returns error for the request, with an object as result value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resultValue"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> Fail<T>(T resultValue, string message = "")
		{
			return RequestSuccess(value: resultValue, isSuccess: false, message: message);
		}

		/// <summary>
		/// Returns error for the request, with an empty result value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> Fail<T>(string message = "")
		{
			return RequestSuccess(value: default(T), isSuccess: false, message: message);
		}

		/// <summary>
		/// Returns error for the request, with a boolean result value
		/// </summary>
		/// <param name="resultValue"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<bool> Fail(bool resultValue = false, string message = "")
		{
			return RequestSuccess(value: resultValue, isSuccess: false, message: message);
		}

		/// <summary>
		/// Returns error for the request, receiving an object from other request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objectReply"></param>
		/// <returns></returns>
		public static IApiResponse<T> Fail<T>(IApiResponse<T> objectReply)
		{
			return PreviousRequestError(objectReply);
		}

		/// <summary>
		/// Returns error for the request, receiving an object from other request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="Z"></typeparam>
		/// <param name="objectReply"></param>
		/// <returns></returns>
		public static IApiResponse<T> Fail<T, Z>(IApiResponse<Z> objectReply)
		{
			return PreviousRequestError<T, Z>(objectReply);
		}

		/// <summary>
		/// Returns a bad request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> BadRequest<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.BadRequest, message);
		}

		/// <summary>
		/// Returns a not found
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> NotFound<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.NotFound, message);
		}

		/// <summary>
		/// Returns an unauthorized
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> Unauthorized<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.Unauthorized, message);
		}

		/// <summary>
		/// Returns an internal server error
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> InternalServerError<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.InternalServerError, message);
		}

		/// <summary>
		/// Returns a request timeout
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> RequestTimeout<T>(string message = "")
		{
			return RequestError<T>(HttpStatusCode.RequestTimeout, message);
		}

		/// <summary>
		/// Returns a generic error
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static IApiResponse<T> GenericError<T>(HttpStatusCode statusCode, string message = "")
		{
			return RequestError<T>(statusCode, message);
		}

		#region :: private ::

		private static IApiResponse<T> RequestSuccess<T>(T? value, bool isSuccess, string message)
		{
			return new ApiResponse<T>
			{
				ResultValue = value,
				Message = message,
				StatusCode = HttpStatusCode.OK,
				IsSuccess = isSuccess
			};
		}

		private static IApiResponse<T> RequestError<T>(HttpStatusCode statusCode, string message)
		{
			return new ApiResponse<T>
			{
				ResultValue = default,
				Message = message,
				StatusCode = statusCode,
				IsSuccess = false
			};
		}

		private static IApiResponse<T> PreviousRequestError<T>(IApiResponse<T> objectReply)
		{
			if (objectReply.StatusCode != HttpStatusCode.OK)
			{
				return RequestError<T>(objectReply.StatusCode, objectReply.Message);
			}

			return RequestSuccess(value: default(T), isSuccess: false, message: objectReply.Message);
		}

		private static IApiResponse<T> PreviousRequestError<T, Z>(IApiResponse<Z> objectReply)
		{
			if (objectReply.StatusCode != HttpStatusCode.OK)
			{
				return RequestError<T>(objectReply.StatusCode, objectReply.Message);
			}

			return RequestSuccess(value: default(T), isSuccess: false, message: objectReply.Message);
		}

		#endregion
	}
}
