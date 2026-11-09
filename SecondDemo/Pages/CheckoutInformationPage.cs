using Microsoft.Playwright;

namespace SecondDemo.Pages;

public sealed class CheckoutInformationPage
{
    private readonly ILocator _firstNameInput;
    private readonly ILocator _lastNameInput;
    private readonly ILocator _postalCodeInput;
    private readonly ILocator _errorMessage;
    private readonly ILocator _cancelButton;
    private readonly ILocator _continueButton;

    public CheckoutInformationPage(IPage page)
    {
        _firstNameInput = page.Locator("[data-test='firstName']");
        _lastNameInput = page.Locator("[data-test='lastName']");
        _postalCodeInput = page.Locator("[data-test='postalCode']");
        _errorMessage = page.Locator(".error-message-container");
        _cancelButton = page.Locator("[data-test='cancel']");
        _continueButton = page.Locator("[data-test='continue']");
    }

    public Task EnterFirstNameAsync(string firstName) => _firstNameInput.FillAsync(firstName);

    public Task EnterLastNameAsync(string lastName) => _lastNameInput.FillAsync(lastName);

    public Task EnterPostalCodeAsync(string postalCode) => _postalCodeInput.FillAsync(postalCode);

    public Task CancelAsync() => _cancelButton.ClickAsync();

    public Task ContinueAsync() => _continueButton.ClickAsync();

    public async Task EnterInformationAsync(string firstName, string lastName, string postalCode)
    {
        await EnterFirstNameAsync(firstName);
        await EnterLastNameAsync(lastName);
        await EnterPostalCodeAsync(postalCode);
    }

    public Task<string?> GetErrorMessageAsync() => _errorMessage.TextContentAsync();
}