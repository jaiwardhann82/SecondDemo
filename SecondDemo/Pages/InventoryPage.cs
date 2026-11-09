using Microsoft.Playwright;

namespace SecondDemo.Pages;

public sealed class InventoryPage
{
    private readonly IPage _page;
    private readonly ILocator _inventoryContainer;
    private readonly ILocator _inventoryItems;
    private readonly CartPage _cartPage;

    public InventoryPage(IPage page)
    {
        _page = page;
        _inventoryContainer = page.Locator("[data-test='inventory-container']");
        _inventoryItems = page.Locator("[data-test='inventory-item']");
        _cartPage = new CartPage(page);
    }

    public Task WaitForPageLoadedAsync() => _inventoryContainer.WaitForAsync(new()
    {
        State = WaitForSelectorState.Visible
    });

    public Task<int> GetProductCountAsync() => _inventoryItems.CountAsync();

    public Task OpenCartAsync() => _cartPage.OpenAsync();

    public Task<int> GetCartItemCountAsync() => _cartPage.GetItemCountAsync();

    public Task AddProductToCartAsync(string productName)
    {
        var productSlug = ToProductSlug(productName);
        return _page.Locator($"[data-test='add-to-cart-{productSlug}']").ClickAsync();
    }

    public Task RemoveProductFromCartAsync(string productName)
    {
        var productSlug = ToProductSlug(productName);
        return _page.Locator($"[data-test='remove-{productSlug}']").ClickAsync();
    }

    public async Task<string?> GetProductCartButtonCaptionAsync(string productName)
    {
        var productSlug = ToProductSlug(productName);
        var removeButton = _page.Locator($"[data-test='remove-{productSlug}']");
        if (await removeButton.CountAsync() > 0)
        {
            return await removeButton.TextContentAsync();
        }

        return await _page.Locator($"[data-test='add-to-cart-{productSlug}']").TextContentAsync();
    }

    public Task ViewProductDetailsAsync(string productName)
    {
        var accessibleName = $"View details for {productName}";
        return _page.GetByRole(AriaRole.Button, new() { Name = accessibleName }).First.ClickAsync();
    }

    private static string ToProductSlug(string productName) =>
        productName.Trim().ToLowerInvariant().Replace(' ', '-');
}