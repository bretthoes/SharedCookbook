# Git branching

This is a single-maintainer repo. Routine work does not use pull requests.

Commit on `dev`. Merge `dev` into `master` only after tests pass. A push to `master` deploys production.

## Branches

| Branch | What it is for |
| ------ | -------------- |
| `dev` | Day-to-day commits. CI runs. Nothing deploys. |
| `master` | Production. Merge `dev` here when a change is ready to release. |

[GitVersion](../.github/GitVersion.yml) also treats `develop` and `development` as the same line as `dev`, and `main` as the same line as `master`. Use `dev` and `master`.

`dev` versions are labeled beta. `master` versions have no prerelease label.

## Day to day

Sync `dev` before committing so the branch matches the remote.

```bash
git checkout dev
git pull --ff-only origin dev
# commit
git push origin dev
```

CI runs on that push and does not deploy. See [github-actions.md](./deploy/github-actions.md).

## Release

Run tests first. Push `master` only when they pass.

```bash
dotnet test
git checkout master
git pull --ff-only origin master
git merge dev
git push origin master
```

That push runs CI again. When CI succeeds, the deploy workflow ships `sharedcookbook-api` to Fly.io.

If tests fail, leave `master` unpushed and fix the problem on `dev`.

Do not force-push `master`.
