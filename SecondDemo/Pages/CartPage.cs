using Microsoft.Playwright;

namespace SecondDemo.Pages;

public sealed class CartPage
{
    private readonly ILocator _shoppingCartLink;
    private readonly ILocator _shoppingCartBadge;
    private readonly ILocator _continueShoppingButton;
    private readonly ILocator _checkoutButton;

    public CartPage(IPage page)
    {
        _shoppingCartLink = page.Locator("[data-test='shopping-cart-link']");
        _shoppingCartBadge = page.Locator("[data-test='shopping-cart-badge']");
        _continueShoppingButton = page.Locator("[data-test='continue-shopping']");
        _checkoutButton = page.Locator("[data-test='checkout']");
    }

    public Task OpenAsync() => _shoppingCartLink.ClickAsync();

    public Task ContinueShoppingAsync() => _continueShoppingButton.ClickAsync();

    public Task CheckoutAsync() => _checkoutButton.ClickAsync();

    public async Task<int> GetItemCountAsync()
    {
        if (await _shoppingCartBadge.CountAsync() == 0)
        {
            return 0;
        }

        var badgeText = await _shoppingCartBadge.TextContentAsync();
        return int.TryParse(badgeText, out var itemCount) ? itemCount : 0;
    }
}