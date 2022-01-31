using System;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;

namespace Security
{
	public static class FunctionContextExtension
	{
		public static ClaimsPrincipal GetUser(this FunctionContext FunctionContext, string RoleType)
		{
			try
			{
				return (ClaimsPrincipal)FunctionContext.Items[RoleType];
			}
			catch (Exception e)
			{
				throw new UnauthorizedAccessException(e.Message);
			}
		}
	}
}
