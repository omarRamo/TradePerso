using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TradePerso.Web.Services;

public sealed class CurrentUserAccessor
{
    private readonly AuthenticationStateProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(
        AuthenticationStateProvider provider,
        IHttpContextAccessor httpContextAccessor)
    {
        _provider = provider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> GetUserIdAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<string?> GetEmailAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Identity?.Name ?? user?.FindFirst(ClaimTypes.Email)?.Value;
    }

    private async Task<ClaimsPrincipal?> GetCurrentUserAsync()
    {
        var httpUser = _httpContextAccessor.HttpContext?.User;
        if (httpUser?.Identity?.IsAuthenticated == true)
        {
            return httpUser;
        }

        try
        {
            var state = await _provider.GetAuthenticationStateAsync();
            return state.User;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
