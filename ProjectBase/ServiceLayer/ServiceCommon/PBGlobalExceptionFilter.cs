using PB.Frameworks.Common.General;
using PB.Frameworks.Resources;
using PB.Services.DataContracts;
using System.Net.Http;
using System.Web.Http.Filters;

namespace ServiceCommon
{
	public class PBGlobalExceptionFilterttribute : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext context)
		{
			if (context.Exception != null)
			{
				HttpResponseMessage response;
				if (context.Exception.Data.Contains(GlobalConsts.EXCEPTION_MESSAGE_NAME_KEY))
				{
					var errorResult = new DCErrorResult
					{
						Success = false,
						ErrorCode = context.Exception.Data[GlobalConsts.EXCEPTION_MESSAGE_NAME_KEY].ToString(),
						ErrorMessage = context.Exception.Message
					};
					response = context.Request.CreateResponse(System.Net.HttpStatusCode.OK, errorResult);
				}
				else
				{
					response = context.Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ServicesResource.INTERNAL_SERVER_ERROR);
				}
				context.Response = response;
			}
		}
	}
}
