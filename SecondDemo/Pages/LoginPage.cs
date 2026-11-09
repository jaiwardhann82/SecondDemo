using Microsoft.Playwright;

namespace SecondDemo.Pages;

public sealed class LoginPage
{
    private readonly IPage _page;
    private readonly ILocator _usernameInput;
    private readonly ILocator _passwordInput;
    private readonly ILocator _loginButton;
    private readonly ILocator _errorMessage;

    public LoginPage(IPage page)
    {
        _page = page;
        _usernameInput = page.Locator("[data-test='username']");
        _passwordInput = page.Locator("[data-test='password']");
        _loginButton = page.Locator("[data-test='login-button']");
        _errorMessage = page.Locator(".error-message-container");
    }

    public Task<IResponse?> NavigateAsync(string url) => _page.GotoAsync(url);

    public Task EnterUsernameAsync(string username) => _usernameInput.FillAsync(username);

    public Task EnterPasswordAsync(string password) => _passwordInput.FillAsync(password);

    public Task SubmitAsync() => _loginButton.ClickAsync();

    public async Task LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await SubmitAsync();
    }

    public Task<string?> GetErrorMessageAsync() => _errorMessage.TextContentAsync();
}