using Microsoft.Playwright;

namespace SecondDemo.Pages;

public sealed class OverviewPage
{
    private readonly ILocator _cartList;
    private readonly ILocator _cartItems;
    private readonly ILocator _quantityLabel;
    private readonly ILocator _descriptionLabel;
    private readonly ILocator _summaryInfo;
    private readonly ILocator _cancelButton;
    private readonly ILocator _finishButton;

    public OverviewPage(IPage page)
    {
        _cartList = page.Locator("[data-test='cart-list']");
        _cartItems = _cartList.Locator("[data-test='inventory-item']");
        _quantityLabel = _cartList.Locator("[data-test='cart-quantity-label']");
        _descriptionLabel = _cartList.Locator("[data-test='cart-desc-label']");
        _summaryInfo = page.Locator(".summary_info");
        _cancelButton = page.Locator("[data-test='cancel']");
        _finishButton = page.Locator("[data-test='finish']");
    }

    public Task<int> GetItemCountAsync() => _cartItems.CountAsync();

    public Task<string?> GetQuantityLabelAsync() => _quantityLabel.TextContentAsync();

    public Task<string?> GetDescriptionLabelAsync() => _descriptionLabel.TextContentAsync();

    public Task<string?> GetPaymentInformationAsync() => GetSummaryTextAsync("payment-info-value");

    public Task<string?> GetShippingInformationAsync() => GetSummaryTextAsync("shipping-info-value");

    public Task<string?> GetPriceTotalLabelAsync() => GetSummaryTextAsync("total-info-label");

    public Task<string?> GetSubtotalAsync() => GetSummaryTextAsync("subtotal-label");

    public Task<string?> GetTaxAsync() => GetSummaryTextAsync("tax-label");

    public Task<string?> GetTotalAsync() => GetSummaryTextAsync("total-label");

    public Task CancelAsync() => _cancelButton.ClickAsync();

    public Task FinishAsync() => _finishButton.ClickAsync();

    public async Task<int> GetItemQuantityAsync(int itemIndex)
    {
        var quantityText = await GetItem(itemIndex).Locator("[data-test='item-quantity']").TextContentAsync();
        return int.TryParse(quantityText, out var quantity) ? quantity : 0;
    }

    public Task<string?> GetItemNameAsync(int itemIndex) =>
        GetItem(itemIndex).Locator("[data-test='inventory-item-name']").TextContentAsync();

    public Task<string?> GetItemDescriptionAsync(int itemIndex) =>
        GetItem(itemIndex).Locator("[data-test='inventory-item-desc']").TextContentAsync();

    public Task<string?> GetItemPriceAsync(int itemIndex) =>
        GetItem(itemIndex).Locator("[data-test='inventory-item-price']").TextContentAsync();

    private Task<string?> GetSummaryTextAsync(string testId) =>
        _summaryInfo.Locator($"[data-test='{testId}']").TextContentAsync();

    private ILocator GetItem(int itemIndex) => _cartItems.Nth(itemIndex);
}