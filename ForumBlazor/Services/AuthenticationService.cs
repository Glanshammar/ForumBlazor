namespace ForumBlazor.Services;

using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

public class AuthenticationService
{
private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthenticationService(HttpClient httpClient, 
                                 AuthenticationStateProvider authStateProvider,
                                 ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var loginModel = new { Username = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            await _localStorage.SetItemAsync("authToken", token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string> GetUserNameAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.Identity.Name;
    }

    public async Task<bool> IsUserAuthenticatedAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity.IsAuthenticated;
    }

    public async Task<bool> IsUserInRoleAsync(string role)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.IsInRole(role);
    }
}

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }

    public void NotifyUserAuthentication(string token)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}