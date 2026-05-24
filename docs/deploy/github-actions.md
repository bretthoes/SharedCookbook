# Auto-deploy via GitHub Actions

Day-to-day work happens on `dev`. **Every push to `master` deploys to production** after CI passes.

## Branch model

| Branch | Purpose |
| ------ | ------- |
| `dev` | Daily commits; CI runs, no deploy |
| `master` | Production; merge from `dev` when ready to release |

## The two workflows

### CI - [.github/workflows/ci.yml](../../.github/workflows/ci.yml)

Runs on push to `dev` or `master`, and on PRs targeting those branches.

This workflow does **not** deploy. It gates the deploy workflow.

### Deploy - [.github/workflows/deploy.yml](../../.github/workflows/deploy.yml)

Triggered when CI finishes successfully on a **push to `master`**.

When deploy runs, it installs `flyctl` and runs `flyctl deploy --remote-only` against the `sharedcookbook-api` app. The only GitHub secret it needs is `FLY_API_TOKEN` — everything else lives in `fly secrets`.

## Typical flows

### Day-to-day development

```bash
git checkout dev
git commit -m "placeholder"
git push origin dev
```

CI runs and tests the change; nothing deploys.

### Release to production

```bash
git checkout master
git merge dev
git push origin master
```

1. CI runs on `master` (~2–5 min).
2. On green, deploy starts automatically.
3. ~5 min later, `https://sharedcookbook-api.fly.dev/health` reflects the new version.

## What's live

```powershell
fly status   --app sharedcookbook-api
fly releases --app sharedcookbook-api
fly logs     --app sharedcookbook-api
```

## Manual deploy

Local `fly deploy` can be used when GitHub Actions is down, you want to test a deploy from a feature branch, or you need to roll back fast. The CI deploy token and personal `fly auth` credentials don't conflict.
