# Features

Deep dives on features that have unusual moving parts (native dependencies, external services, non-obvious code paths). Plain CRUD features don't live here - they're self-explanatory in the source.

| Doc | What it covers |
|-----|----------------|
| [recipe-from-photo.md](./recipe-from-photo.md) | Photo -> recipe draft; Tesseract OCR and native deploy notes |
| [recipe-from-url.md](./recipe-from-url.md) | URL -> recipe draft; Spoonacular extract and optional S3 image re-host |
| [recipe-from-voice.md](./recipe-from-voice.md) | Transcript -> recipe draft; OpenAI parsing (STT is on the client) |
| [social-sign-in.md](./social-sign-in.md) | Google / Apple / Facebook token login and bearer session |
| [cookbook-share-link.md](./cookbook-share-link.md) | Shareable cookbook invite links (hashed tokens vs email invites) |
| [subscriptions.md](./subscriptions.md) | Pro subscription via RevenueCat; tier-aware rate limiting; webhook lifecycle |
