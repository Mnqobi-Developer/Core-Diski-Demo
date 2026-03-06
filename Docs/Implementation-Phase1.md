# Football Shirts Store - Admin-First Catalog Setup

## What changed
- Removed all dummy seed data from `ApplicationDbContext`.
- Added admin authentication flow (`/Account/Login`) using ASP.NET Core Identity.
- Added automatic admin role/user seeding from configuration (`AdminSeed` section in `appsettings.json`).
- Added an admin dashboard and simple Bootstrap UI to add:
  - Leagues
  - Clubs
  - Brands
  - Categories
- Added admin jersey management screens:
  - List products
  - Create product
  - Edit product
  - Delete product

## Why this helps
The store now starts with an empty catalog so administrators can enter real soccer jerseys and master data directly from the UI, instead of relying on dummy records.

## Default admin credentials
Configure these in `appsettings.json`:

```json
"AdminSeed": {
  "Email": "admin@footballstore.com",
  "Password": "Admin123!"
}
```

## Expected flow
1. Start app.
2. Login at `/Account/Login`.
3. Go to `Admin > Catalog Setup` and add leagues, clubs, brands, categories.
4. Go to `Admin > Manage Jerseys` and add products.
