Feature: SauceDemo checkout
  As a shopper
  I want to purchase products
  So that I can verify the checkout flow

  Scenario: Purchase five products and return home
    Given I open the SauceDemo login page
    When I log in with username "standard_user" and password "secret_sauce"
    And I add each product and verify its button changes to Remove
      | Product              |
      | Sauce Labs Backpack  |
      | Sauce Labs Bike Light|
      | Sauce Labs Bolt T-Shirt|
      | Sauce Labs Fleece Jacket|
      | Sauce Labs Onesie     |
    And I open the cart and proceed to checkout
    And I enter checkout information with first name "jai", last name "weardhan", and zip code "1025"
    And I continue to the order overview
    Then every cart item has quantity 1 and a description
    When I finish the order
    Then the order confirmation is displayed
    When I return to the home page
    Then the inventory page is displayed