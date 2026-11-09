using Microsoft.Playwright;

namespace SecondDemo.Pages;

public sealed class CheckoutCompletePage
{
    private readonly ILocator _completeContainer;
    private readonly ILocator _ponyExpressImage;
    private readonly ILocator _completeHeader;
    private readonly ILocator _completeText;
    private readonly ILocator _backToProductsButton;
    private readonly ILocator _generatePdfOrderButton;

    public CheckoutCompletePage(IPage page)
    {
        _completeContainer = page.Locator("[data-test='checkout-complete-container']");
        _ponyExpressImage = _completeContainer.Locator("[data-test='pony-express']");
        _completeHeader = _completeContainer.Locator("[data-test='complete-header']");
        _completeText = _completeContainer.Locator("[data-test='complete-text']");
        _backToProductsButton = _completeContainer.Locator("[data-test='back-to-products']");
        _generatePdfOrderButton = _completeContainer.Locator("[data-test='generate-pdf-order']");
    }

    public Task<bool> IsVisibleAsync() => _completeContainer.IsVisibleAsync();

    public Task WaitForPageLoadedAsync() => _completeContainer.WaitForAsync(new()
    {
        State = WaitForSelectorState.Visible
    });

    public Task<bool> IsPonyExpressVisibleAsync() => _ponyExpressImage.IsVisibleAsync();

    public Task<string?> GetConfirmationMessageAsync() => _completeHeader.TextContentAsync();

    public Task<string?> GetDispatchMessageAsync() => _completeText.TextContentAsync();

    public Task BackHomeAsync() => _backToProductsButton.ClickAsync();

    public Task GeneratePdfOrderAsync() => _generatePdfOrderButton.ClickAsync();
}