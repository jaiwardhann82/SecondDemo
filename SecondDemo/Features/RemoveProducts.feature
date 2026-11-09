Feature: Remove products from the cart
  As a shopper
  I want to remove products I no longer want
  So that they are no longer in my cart

  Scenario: Remove two products from the cart
    Given I open the SauceDemo login page
    When I log in with username "standard_user" and password "secret_sauce"
    And I add each product and verify its button changes to Remove
      | Product             |
      | Sauce Labs Backpack |
      | Sauce Labs Onesie   |
    When I remove each selected product and verify its button changes to Add to cart
    Then the cart is empty