using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
	private ClaimsPrincipal? User => accessor.HttpContext?.User;

	public Guid? UserId
	{
		get
		{
			string input = User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
			Guid result;
			return Guid.TryParse(input, out result) ? new Guid?(result) : ((Guid?)null);
		}
	}

	public string? Email => User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

	public string? FullName => User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

	public bool IsAuthenticated
	{
		get
		{
			ClaimsPrincipal? user = User;
			return user != null && user.Identity?.IsAuthenticated == true;
		}
	}
}
