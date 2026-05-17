# Social sign-in

Users sign in with **Google**, **Apple**, or **Facebook** from the mobile app. Each provider returns a token to the client; the API validates that token, finds or creates a local user, and issues an **ASP.NET Core Identity bearer** session (same scheme as password login).

There is no separate “OAuth redirect” flow in this API — the app obtains provider tokens natively and posts them to dedicated login endpoints.

## Flow

1. Mobile SDK completes provider sign-in and yields an ID token (Google/Apple) or access token (Facebook).
2. Client calls one of:
   - `POST /api/users/login-google`
   - `POST /api/users/login-apple`
   - `POST /api/users/login-facebook`
3. Command handler ([LoginWithGoogle](../../src/Application/Users/Commands/LoginWithGoogle/LoginWithGoogle.cs), etc.) calls [ExternalLoginService](../../src/Infrastructure/Identity/ExternalLoginService.cs).
4. Service validates the token with the provider, then **find or create** user:
   - Existing external login → return user id
   - Existing email, new provider → link login
   - New user → create account with confirmed email and random password (never shown to user)
5. [SignInPrincipalFactory](../../src/Infrastructure/Identity/SignInPrincipalFactory.cs) builds claims; endpoint returns `Results.SignIn` with `IdentityConstants.BearerScheme`.

Endpoints are in [Users.cs](../../src/Web/Endpoints/Users.cs). Standard Identity registration/password routes also exist via `MapIdentityApi<ApplicationUser>()`.

## Provider validation (where complexity lives)

All logic is centralized in [ExternalLoginService.cs](../../src/Infrastructure/Identity/ExternalLoginService.cs):

| Provider | Validation approach |
|----------|---------------------|
| Google | `GoogleJsonWebSignature.ValidateAsync` — audience must match configured ClientId |
| Apple | JWT validated against Apple’s OIDC metadata (`appleid.apple.com`); audience is app bundle id |
| Facebook | `debug_token` to verify app + token, then Graph API `/me` for id and email |

Email is required from every provider; missing email fails login.

## Configuration

Per-provider options under `Authentication` in config (see [appsettings.Development.json](../../src/Web/appsettings.Development.json) for shape):

- `Authentication:Google:ClientId`
- `Authentication:Apple:BundleId`
- `Authentication:Facebook:AppId` / `AppSecret`

Production values are Fly secrets (`Authentication__Google__ClientId`, etc.) — [fly.md](../deploy/fly.md#secrets).

**The ClientId / bundle id on the server must match what the mobile app uses** when requesting tokens. Mismatch is the most common “invalid token” failure in dev.

## Where to look in code

| Area | Path |
|------|------|
| Endpoints | [src/Web/Endpoints/Users.cs](../../src/Web/Endpoints/Users.cs) |
| Commands | [src/Application/Users/Commands/LoginWith*/](../../src/Application/Users/Commands/) |
| Token validation + user provisioning | [ExternalLoginService.cs](../../src/Infrastructure/Identity/ExternalLoginService.cs) |
| Options types | [GoogleAuthOptions](../../src/Infrastructure/Identity/GoogleAuthOptions.cs), [AppleAuthOptions](../../src/Infrastructure/Identity/AppleAuthOptions.cs), [FacebookAuthOptions](../../src/Infrastructure/Identity/FacebookAuthOptions.cs) |
