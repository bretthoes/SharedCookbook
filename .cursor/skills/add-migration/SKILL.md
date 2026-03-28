---
name: add-migration
description: >-
  Guides an agent through adding a new EF Core migration to the SharedCookbook
  project, including editing domain entities, running the EF migration command,
  and applying the custom zero-padded numeric prefix naming convention (renaming
  generated files and updating the Migration attribute). Use when adding a new
  EF Core migration, changing a domain entity's schema, adding a new entity to
  the database, or when the user mentions "migration", "EF Core", "entity
  change", or "database schema".
---

# Adding an EF Core Migration

## Project Layout

| Path | Purpose |
|------|---------|
| `src/Domain/Entities/` | Domain entities — sealed C# classes inheriting `BaseAuditableEntity` |
| `src/Infrastructure/Data/ApplicationDbContext.cs` | One `DbSet<T>` per entity |
| `src/Infrastructure/Data/Migrations/` | All migration files |
| `src/Infrastructure/Infrastructure.csproj` | Contains EF Tools; run commands from here |
| `src/Web/` | Startup project used for `--startup-project` flag |

## Workflow

Copy this checklist and track progress:

```
- [ ] Step 1: Understand the change
- [ ] Step 2: Edit the entity
- [ ] Step 3: Update DbContext (new entities only)
- [ ] Step 4: Run EF migration command
- [ ] Step 5: Determine next prefix
- [ ] Step 6: Rename generated files
- [ ] Step 7: Update Migration attribute
- [ ] Step 8: Verify
```

---

### Step 1: Understand the Change

Confirm with the user (or derive from context):
- Which entity/entities need modification
- The migration name: PascalCase, descriptive (e.g. `AddServingsToRecipe`, `CreateNotificationTable`)

---

### Step 2: Edit the Entity

Modify the relevant file(s) in `src/Domain/Entities/`.

**Conventions:**
- Classes are `sealed` and inherit `BaseAuditableEntity`
- Use C# properties with appropriate nullability (`string?`, `int?`, etc.)
- No data annotations — EF configuration is applied via `IEntityTypeConfiguration<T>` or by convention
- Navigation properties use `init` accessors; collections initialize to `[]`

**Example — adding a nullable property:**
```csharp
public int? Servings { get; set; }
```

**Example — new entity:**
```csharp
public sealed class CookbookNotification : BaseAuditableEntity
{
    public int CookbookId { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }
}
```

---

### Step 3: Update DbContext (New Entities Only)

If a brand-new entity class was added, open `src/Infrastructure/Data/ApplicationDbContext.cs` and add a `DbSet<T>` following the expression-bodied pattern:

```csharp
public DbSet<CookbookNotification> CookbookNotifications => Set<CookbookNotification>();
```

Skip this step if only modifying an existing entity.

---

### Step 4: Run the EF Migration Command

From `src/Infrastructure/`, run:

```bash
dotnet ef migrations add [MigrationName] --startup-project ../Web
```

> **If the command fails** (e.g. missing database connection string): the startup project's `appsettings.Development.json` must have a valid connection string. Alternatively, try the `--no-build` flag if the project is already compiled. Migration files are generated without needing a live database connection when using a design-time factory.

The command generates three files in `src/Infrastructure/Data/Migrations/`:
- `[efTimestamp]_[MigrationName].cs`
- `[efTimestamp]_[MigrationName].Designer.cs`
- Updates `ApplicationDbContextModelSnapshot.cs` in-place (no rename needed)

---

### Step 5: Determine the Next Prefix

Scan `src/Infrastructure/Data/Migrations/` for `*.cs` files. Exclude:
- Files ending in `.Designer.cs`
- `ApplicationDbContextModelSnapshot.cs`

Find the highest 14-digit numeric prefix among the remaining files. Add 1 and zero-pad to 14 digits.

**Example:**
```
00000000000013_NextUpdate.cs  ← current highest = 13
next prefix = 00000000000014
```

---

### Step 6: Rename Generated Files

Rename both EF-generated files, replacing the EF timestamp with the new prefix:

```
[efTimestamp]_[MigrationName].cs          →  [newPrefix]_[MigrationName].cs
[efTimestamp]_[MigrationName].Designer.cs →  [newPrefix]_[MigrationName].Designer.cs
```

**Concrete example** (migration name `AddServingsToRecipe`, EF timestamp `20260327123456`, new prefix `00000000000014`):

```
20260327123456_AddServingsToRecipe.cs          →  00000000000014_AddServingsToRecipe.cs
20260327123456_AddServingsToRecipe.Designer.cs →  00000000000014_AddServingsToRecipe.Designer.cs
```

---

### Step 7: Update the Migration Attribute

Inside the renamed `.Designer.cs` file, find the `[Migration("...")]` attribute near the top of the file and update its value to use the new prefix.

**Before:**
```csharp
[Migration("20260327123456_AddServingsToRecipe")]
```

**After:**
```csharp
[Migration("00000000000014_AddServingsToRecipe")]
```

> Only change the string value inside `[Migration("...")]`. Do not modify any other auto-generated content in `.Designer.cs`.

---

### Step 8: Verify

Confirm all of the following:

- [ ] `[newPrefix]_[MigrationName].cs` exists in `src/Infrastructure/Data/Migrations/`
- [ ] `[newPrefix]_[MigrationName].Designer.cs` exists in `src/Infrastructure/Data/Migrations/`
- [ ] No files with the old EF timestamp prefix remain
- [ ] `[Migration("...")]` attribute in `.Designer.cs` uses the new prefix
- [ ] `ApplicationDbContextModelSnapshot.cs` reflects the schema changes (updated automatically by EF)
