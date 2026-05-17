# Auto-deploy via GitHub Actions

Every push to `master` deploys to Fly automatically, but **only if CI passes first**.

## The two workflows

### CI — [.github/workflows/ci.yml](../../.github/workflows/ci.yml)

Existing workflow. Runs on push to `master` and on PRs.

- Checks out source, sets up .NET 10 SDK.
- Runs GitVersion (and tags `master` commits with the computed SemVer).
- `dotnet build` and `dotnet test` with `SkipNSwag=true`.
- Creates a GitHub Release if a tag was pushed.

This workflow does **not** deploy. It just gates the deploy workflow.

### Deploy — [.github/workflows/deploy.yml](../../.github/workflows/deploy.yml)

Triggered by the CI workflow finishing on `master`. If CI passed, it installs `flyctl` and runs `flyctl deploy --remote-only` against the `sharedcookbook-api` app. The only secret it needs from GitHub is `FLY_API_TOKEN` — everything else lives in `fly secrets`.

## Typical flow

1. **Actions tab** → "CI" runs (≈ 2-5 min).
2. On green, "Deploy to Fly.io" starts automatically.
3. It runs `flyctl deploy --remote-only`, which:
  - Sends the source to a Fly remote builder.
  - Builds the Docker image from [Dockerfile](../../Dockerfile).
  - Rolls out new machines, waiting for the `/health` check to pass before killing the old ones.
4. ~5 mins later, `https://sharedcookbook-api.fly.dev/health` reflects the new version.

Verify locally:

```powershell
fly status   --app sharedcookbook-api
fly releases --app sharedcookbook-api
fly logs     --app sharedcookbook-api
```

## Manual deploy

Local `fly deploy`can be used when GitHub Actions is down, you want to test a deploy from a feature branch, or you need to roll back fast. The CI deploy token and personal `fly auth` credentials don't conflict.



