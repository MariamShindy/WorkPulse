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
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
    public string? FullName => User?.FindFirstValue(ClaimTypes.Name);
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
}
