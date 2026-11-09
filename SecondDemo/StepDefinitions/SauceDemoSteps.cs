using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using SecondDemo.Pages;

namespace SecondDemo.StepDefinitions;

[Binding]
public sealed class SauceDemoSteps
{
    private const string BaseUrl = "https://www.saucedemo.com/";
    private readonly ScenarioContext _scenarioContext;
    private readonly List<string> _selectedProducts = [];

    public SauceDemoSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private IPage Page => (IPage)_scenarioContext["Page"];

    [Given("I open the SauceDemo login page")]
    public async Task GivenIOpenTheSauceDemoLoginPage()
    {
        await new LoginPage(Page).NavigateAsync(BaseUrl);
    }

    [When("I log in with username {string} and password {string}")]
    public async Task WhenILogIn(string username, string password)
    {
        await new LoginPage(Page).LoginAsync(username, password);
    }

    [When("I add each product and verify its button changes to Remove")]
    public async Task WhenIAddEachProductAndVerifyItsButtonChangesToRemove(Table products)
    {
        var inventoryPage = new InventoryPage(Page);
        _selectedProducts.Clear();

        foreach (var row in products.Rows)
        {
            var productName = row["Product"];
            await inventoryPage.AddProductToCartAsync(productName);

            var buttonCaption = await inventoryPage.GetProductCartButtonCaptionAsync(productName);
            Assert.That(buttonCaption?.Trim(), Is.EqualTo("Remove"),
                $"The cart button for '{productName}' should change to Remove immediately after adding it.");

            _selectedProducts.Add(productName);
        }
    }

    [When("I remove each selected product and verify its button changes to Add to cart")]
    public async Task WhenIRemoveEachSelectedProductAndVerifyItsButtonChangesToAddToCart()
    {
        var inventoryPage = new InventoryPage(Page);

        foreach (var productName in _selectedProducts.ToArray())
        {
            await inventoryPage.RemoveProductFromCartAsync(productName);

            var buttonCaption = await inventoryPage.GetProductCartButtonCaptionAsync(productName);
            Assert.That(buttonCaption?.Trim(), Is.EqualTo("Add to cart"),
                $"The cart button for '{productName}' should change to Add to cart immediately after removal.");
        }

        _selectedProducts.Clear();
    }

    [Then("the cart is empty")]
    public async Task ThenTheCartIsEmpty()
    {
        Assert.That(await new CartPage(Page).GetItemCountAsync(), Is.Zero);
    }

    [When("I open the cart and proceed to checkout")]
    public async Task WhenIOpenTheCartAndProceedToCheckout()
    {
        var inventoryPage = new InventoryPage(Page);
        await inventoryPage.OpenCartAsync();

        var cartPage = new CartPage(Page);
        Assert.That(await cartPage.GetItemCountAsync(), Is.EqualTo(_selectedProducts.Count));
        await cartPage.CheckoutAsync();
    }

    [When("I enter checkout information with first name {string}, last name {string}, and zip code {string}")]
    public async Task WhenIEnterCheckoutInformation(string firstName, string lastName, string zipCode)
    {
        await new CheckoutInformationPage(Page).EnterInformationAsync(firstName, lastName, zipCode);
    }

    [When("I continue to the order overview")]
    public async Task WhenIContinueToTheOrderOverview()
    {
        await new CheckoutInformationPage(Page).ContinueAsync();
    }

    [Then("every cart item has quantity 1 and a description")]
    public async Task ThenEveryCartItemHasQuantityOneAndADescription()
    {
        var overviewPage = new OverviewPage(Page);
        Assert.That(await overviewPage.GetQuantityLabelAsync(), Is.EqualTo("QTY"));
        Assert.That(await overviewPage.GetDescriptionLabelAsync(), Is.EqualTo("Description"));
        Assert.That(await overviewPage.GetItemCountAsync(), Is.EqualTo(_selectedProducts.Count));

        var actualProductNames = new List<string?>();
        var itemCount = await overviewPage.GetItemCountAsync();

        for (var itemIndex = 0; itemIndex < itemCount; itemIndex++)
        {
            var productName = await overviewPage.GetItemNameAsync(itemIndex);
            var description = await overviewPage.GetItemDescriptionAsync(itemIndex);

            Assert.That(await overviewPage.GetItemQuantityAsync(itemIndex), Is.EqualTo(1),
                $"'{productName}' should have a quantity of 1.");
            Assert.That(description?.Trim(), Is.Not.Null.And.Not.Empty,
                $"'{productName}' should have a product description.");

            actualProductNames.Add(productName?.Trim());
        }

        Assert.That(actualProductNames, Is.EquivalentTo(_selectedProducts));
    }

    [When("I finish the order")]
    public async Task WhenIFinishTheOrder()
    {
        await new OverviewPage(Page).FinishAsync();
    }

    [Then("the order confirmation is displayed")]
    public async Task ThenTheOrderConfirmationIsDisplayed()
    {
        var completePage = new CheckoutCompletePage(Page);
        await completePage.WaitForPageLoadedAsync();
        Assert.That(await completePage.IsVisibleAsync(), Is.True);
        Assert.That(await completePage.GetConfirmationMessageAsync(), Is.EqualTo("Thank you for your order!"));
    }

    [When("I return to the home page")]
    public async Task WhenIReturnToTheHomePage()
    {
        await new CheckoutCompletePage(Page).BackHomeAsync();
    }

    [Then("the inventory page is displayed")]
    public async Task ThenTheInventoryPageIsDisplayed()
    {
        var inventoryPage = new InventoryPage(Page);
        await inventoryPage.WaitForPageLoadedAsync();
        Assert.That(await inventoryPage.GetProductCountAsync(), Is.EqualTo(6));
    }
}