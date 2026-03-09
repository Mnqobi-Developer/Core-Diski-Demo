# Football Shirts Store - Current Implementation Snapshot

## Admin capabilities
- Admin login with ASP.NET Core Identity.
- Admin dashboard and catalog setup pages for leagues, clubs, brands, categories.
- Admin jersey CRUD with image upload support.

## Customer shopping flow (new)
- Public product catalog page with search and filters:
  - league, club, brand, category/shirt type, size, min/max price.
- Product details page with stock visibility and add-to-cart action.
- Session-based cart for customers:
  - add/update/remove items,
  - stock-aware quantity checks.
- Checkout + confirmation flow with South African payment options:
  - PayFast
  - Ozow
  - Yoco
  - Peach Payments
  - Manual EFT

## Data seeding
- Master lookup data seeded for:
  - major European leagues,
  - South African Betway Premiership,
  - football kit brands,
  - categories,
  - clubs,
  - standard sizes.

## Important note
This environment does not include the `dotnet` SDK, so migrations and runtime validation must be executed locally.
