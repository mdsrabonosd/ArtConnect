# ArtConnect

A social art marketplace and custom painting commission platform, built with ASP.NET Core MVC (.NET 10), Entity Framework Core and SQL Server using a Clean Architecture layout.

This repository contains the **initial development phase**: architecture, database model, Identity, repository and facade layers, login, role-based access and an admin panel with basic CRUD.

## Solution structure

```
src/
  ArtConnect.Framework    Shared helpers: roles, URL constants, extensions, OperationResult
  ArtConnect.Entities     Domain entities, enums, ApplicationUser / ApplicationRole (Identity)
  ArtConnect.DataAccess   ApplicationDbContext, generic repository, repositories, seeder, Migrations
  ArtConnect.Facade       Business logic (one facade per module) + DependencyInjection.cs
  ArtConnect.Web.UI       ASP.NET Core MVC app: login, public pages and the Admin area
```

Dependency direction: `Web.UI -> Facade -> DataAccess -> Entities -> Framework`

## What is included

- Generic repository (`IRepository<T>` / `Repository<T>`) with soft delete (`IsDeleted`) and a global query filter
- One repository and one facade for each of the 14 entities in the ER diagram
- ASP.NET Core Identity with integer keys, roles `Admin`, `Artist`, `Customer`
- Seeded roles, default admin account and 8 default categories
- Login, logout, **registration** (Customer or Artist, auto sign-in), **My profile** and **Change password**, access denied page
- Blocked or deleted users cannot sign in; Admin is never available on the public registration form
- Role-based authorization: every page in the Admin area needs the `Admin` role
- Admin panel: layout, navigation, dashboard
- **Public site:** home page, gallery with search, category, medium and price filters and paging, artwork details, artist list and portfolios, likes, comments and wishlist
- **Customer area** (`/Customer`): dashboard, post custom requests, compare and accept proposals, orders with progress photos, reviews, wishlist, cart and checkout (payment simulated)
- **Artist area** (`/Artist`): dashboard, own artworks (upload, edit, delete), open requests, send and withdraw proposals, orders (start, progress photos, deliver), profile
- Full CRUD (insert, update, soft delete, get all, get by id) in the admin panel for **Users, Artists, Artworks, Categories, Custom requests, Proposals, Orders, Comments and Reviews**

## Setup

1. **Requirements:** .NET 10 SDK, SQL Server Express (`.\SQLEXPRESS`), Visual Studio 2022 or newer.
2. **Connection string** is in `src/ArtConnect.Web.UI/appsettings.json`:
   `Server=.\SQLEXPRESS;Database=ArtConnectDb;Trusted_Connection=True;TrustServerCertificate=True;`
3. **Create the database.** In Visual Studio open *Package Manager Console*, set *Default project* to `ArtConnect.DataAccess`, then run:

   ```
   Add-Migration InitialCreate -Project ArtConnect.DataAccess -StartupProject ArtConnect.Web.UI
   Update-Database -Project ArtConnect.DataAccess -StartupProject ArtConnect.Web.UI
   ```

   Or with the CLI from the repository root:

   ```
   dotnet ef migrations add InitialCreate --project src/ArtConnect.DataAccess --startup-project src/ArtConnect.Web.UI
   dotnet ef database update --project src/ArtConnect.DataAccess --startup-project src/ArtConnect.Web.UI
   ```

4. **Run** `ArtConnect.Web.UI`. On start the app seeds the roles, the admin account and the categories.

## Default admin login

| Email | Password |
| --- | --- |
| `admin@artconnect.com` | `Admin@12345` |

Change these in the `SeedAdmin` section of `appsettings.json` before the first run (or change the password inside the app later). The seeder only creates the admin if the email does not exist yet.

Sign in at `/login`. Admins go to `/Admin/Dashboard`.

## Public pages

| URL | Purpose |
| --- | --- |
| `/register` | Create a Customer or Artist account (artists also write a short bio and get an artist profile) |
| `/login` | Sign in. Admin goes to the admin panel, others go to the home page |
| `/profile` | Update name and phone number |
| `/change-password` | Change password |
| `/Gallery` | Browse and search paintings (12 per page) |
| `/Artists` | Artist list and public portfolios |
| `/Customer/Dashboard` | Customer space (customers only) |
| `/Artist/Dashboard` | Artist studio (artists only) |
| `/Admin/Dashboard` | Admin panel (admin only) |

## Try the full workflow (end to end)

1. Register an **Artist** at `/register`, then log in and upload a painting in **My artworks**.
2. Register a **Customer** in a private window. Like, comment and save the painting in the gallery, add it to the cart and place the order.
3. As the customer, post a **custom request**. As the artist, open **Open requests** and send a proposal.
4. As the customer, open the request and **accept** the proposal. An order is created.
5. As the artist, open the order, **Start work**, upload progress photos and **Mark as delivered**.
6. As the customer, open the order and leave a **review**. The artist sees the rating on the dashboard and the public portfolio.
7. As admin, verify the artist and watch all the data in the admin panel.

## Try the admin CRUD flow

1. **Users > Add user**: create a user with the `Artist` role.
2. **Artists > Add artist profile**: attach a profile to that user, then use the verify button.
3. **Artworks > Add artwork**: choose the artist and category, upload a JPG or PNG.
4. **Custom requests**: add a request for a customer. **Proposals**: add an offer from the artist. **Orders**: create an order and set it to Delivered. **Reviews**: add a review for that delivered order. **Comments**: moderate comments on artworks.
5. Edit, view and delete from each list. Delete is a soft delete, so rows stay in the database with `IsDeleted = 1`.

## Business rules already enforced

- Category names are unique; a category used by artworks or custom requests cannot be deleted
- An artist with artworks cannot be deleted; new artist profiles start as not verified
- Price must be greater than zero; artworks need at least one image (JPG/PNG, max 5 MB)
- The admin cannot block, delete or demote their own account
- Passwords: at least 8 characters with a capital letter, a small letter and a number
- Blocking a user also signs them out of open sessions

## Packages and vulnerability warnings

`Microsoft.EntityFrameworkCore.SqlServer` depends on `Microsoft.Data.SqlClient`, which still resolves `System.Security.Cryptography.Pkcs` 9.0.11 (advisories GHSA-846f-f764-76mx and GHSA-2fmv-rv2c-59x7). The projects `ArtConnect.DataAccess`, `ArtConnect.Facade` and `ArtConnect.Web.UI` therefore reference the patched `System.Security.Cryptography.Pkcs` **10.0.13** directly. To check:

```
dotnet restore
dotnet list package --vulnerable --include-transitive
```

When a newer SqlClient fixes this, the three `System.Security.Cryptography.Pkcs` lines can be removed.

## Next phases

Follows and a personal art feed, chat between customer and artist, notifications, a real payment gateway, artist matching and recommendations, and forgot-password by email.

## Notes

- Bootstrap, Bootstrap Icons, jQuery validation and the web fonts load from a CDN, so the admin panel needs an internet connection in development.
- Uploaded images are saved in `src/ArtConnect.Web.UI/wwwroot/uploads/` and are ignored by Git.
