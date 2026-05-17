# Recipe from photo

The mobile app lets a user take a picture of a recipe (a cookbook page, a handwritten card, a screenshot). The API extracts text from the image, parses it into a recipe shape, and returns a draft the client can edit before saving.

Implemented with **Tesseract OCR** via [IOcrService](../../src/Application/Common/Interfaces/IOcrService.cs) (`TesseractOcrService`). This is one of the few capabilities in the codebase that pulls in **native (non-managed) dependencies**, which is why it has its own doc.

## API and application flow

### Endpoint `POST /api/recipes/parse-recipe-img`

[src/Web/Endpoints/Recipes.cs](../../src/Web/Endpoints/Recipes.cs)

```csharp
builder.MapPost(ParseFromImage, pattern: "/parse-recipe-img").RequireAuthorization();
// ...
private static Task<CreateRecipeDto> ParseFromImage(ISender sender, [FromForm] IFormFile file)
    => sender.Send(new ParseRecipeFromImageCommand(file));
```

### Command + handler

[src/Application/Recipes/Commands/ParseRecipeFromImage/ParseRecipeFromImage.cs](../../src/Application/Recipes/Commands/ParseRecipeFromImage/ParseRecipeFromImage.cs)

The handler runs OCR, then parses the resulting plain text into a `CreateRecipeDto`. Parsing is **deliberately simple** — string splits and one regex — because the user gets to review and edit the result on the mobile side before save:

- **Title**: first non-empty line.
- **Ingredients**: lines matching `^- (.+?)( \(optional\))?$`. A leading `-`  is required; `(optional)` is captured into the ingredient's `Optional` flag.
- **Directions**: every non-empty line after a line that starts with `Directions`, `Steps`, or `Instructions` (case-insensitive).
- Everything else (`Summary`, times, servings, images, cookbook id) is left blank and filled in by the user in the app.

There is a `// TODO refactor parsing logic to a separate service` note — the parsing is fine inline for now but could move to its own class if it grows.

## OCR implementation (Tesseract)

### Service

[src/Infrastructure/Ocr/TesseractOcrService.cs](../../src/Infrastructure/Ocr/TesseractOcrService.cs)

```csharp
private static readonly string DataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");
private const string Language = "eng";

using var engine = new TesseractEngine(DataPath, Language, EngineMode.Default);
```

- Registered as `AddTransient<IOcrService, TesseractOcrService>()` in [src/Infrastructure/DependencyInjection.cs](../../src/Infrastructure/DependencyInjection.cs).
- A new `TesseractEngine` is created per request. Tesseract engines hold native memory; reusing them between requests would require pooling and care around thread safety. Transient + `using` is the simplest correct choice for our load.
- Reads the entire uploaded stream into memory and passes the bytes to `Pix.LoadFromMemory`. Fine for phone photos (a few MB); would need streaming/limits if we ever accept large scans.
- Only English (`eng`) is configured.

### NuGet packages

[Directory.Packages.props](../../Directory.Packages.props) → [src/Infrastructure/Infrastructure.csproj](../../src/Infrastructure/Infrastructure.csproj):


| Package             | Version | Purpose                                        |
| ------------------- | ------- | ---------------------------------------------- |
| `Tesseract`         | 5.2.0   | Managed P/Invoke wrapper around `libtesseract` |
| `Tesseract.Drawing` | 5.2.0   | `Pix` adapters for `System.Drawing` types      |


These NuGets do **not** ship the Tesseract C library or language data — they only call out to system-installed natives. That's what makes the Dockerfile a load-bearing part of this feature.

### Language data (`tessdata`)

[src/Web/wwwroot/tessdata/](../../src/Web/wwwroot/tessdata/)

`TesseractEngine` needs a folder with `<lang>.traineddata` files. We point it at `wwwroot/tessdata`, which gets copied into the publish output as static web content.

**The English trained-data file (`eng.traineddata`) is required at runtime and must be present in `src/Web/wwwroot/tessdata/` before the Docker image is built.** Grab it from one of the official Tesseract repos and drop it into that folder:

- [tessdata_fast](https://github.com/tesseract-ocr/tessdata_fast) — smaller / faster (~2 MB)
- [tessdata](https://github.com/tesseract-ocr/tessdata) — middle ground
- [tessdata_best](https://github.com/tesseract-ocr/tessdata_best) — slowest / most accurate

If the file is missing, `TesseractOcrService.ExtractText` will throw `DirectoryNotFoundException` (or Tesseract will fail to load the language) on the first OCR request — health checks still pass, so you only find out when someone scans a recipe.

The folder also contains a `.gitmodules` referencing `tessconfigs`, which is unused at runtime today; safe to ignore.

### Native dependencies (Docker)

From [Dockerfile](../../Dockerfile) (runtime stage):

```dockerfile
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        tesseract-ocr \
        libleptonica-dev \
        libtesseract-dev \
    && rm -rf /var/lib/apt/lists/*

RUN ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so
WORKDIR /app/x64
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5  /app/x64/libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so
```

What each piece does:


| Step                                            | Why                                                                                                                                                                                                 |
| ----------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Install `tesseract-ocr`                         | The CLI/runtime. Also pulls Leptonica as a dep. We don't shell out to the binary — but it's the easiest way to get the runtime libraries on the system.                                             |
| Install `libleptonica-dev` / `libtesseract-dev` | Provides the `.so` files the .NET wrapper P/Invokes into.                                                                                                                                           |
| `libdl.so` symlink                              | Some Debian images ship only `libdl.so.2`. A few packages compiled against the unversioned `libdl.so` symbol. The symlink keeps the .NET P/Invoke happy.                                            |
| `/app/x64/libleptonica-1.82.0.so`               | The `Tesseract` NuGet wrapper looks for Leptonica at this **exact filename** under `x64/`. Debian's `liblept.so.5` is the same binary; the symlink renames it where the wrapper expects to find it. |
| `/app/x64/libtesseract50.so`                    | Same story for `libtesseract`. The wrapper wants the file named `libtesseract50.so`.                                                                                                                |


If the wrapper version changes (e.g. NuGet `Tesseract` bumps a major version) the expected filenames may change. Symptoms: `DllNotFoundException` or `Failed to load library` at engine construction time. Fix is to update the `ln -s` targets to match what the new wrapper wants.

The published .NET app is copied into `/app`, so `wwwroot/tessdata/eng.traineddata` ends up at `/app/wwwroot/tessdata/eng.traineddata`, which is what `Directory.GetCurrentDirectory()` resolves to at runtime.

### Operational gotchas

- **Image size is up.** The runtime layer is noticeably bigger because of `tesseract-ocr` + leptonica + dev headers. If we ever care about cold-start latency, the dev packages could be swapped for the runtime-only `libtesseract5` / `libleptonica5` packages.
- **Memory.** Tesseract is the most memory-hungry thing in this process. 512 MB on Fly works for one request at a time but could be tight under concurrent scans. If we see OOMs, scale `[[vm]] memory` in [fly.toml](../../fly.toml).
- **Cold starts.** The Fly machine sleeps when idle (`auto_stop_machines = "stop"`). The first scan after sleep pays both the VM wake (~1-2 s) and the Tesseract engine initialization. Fine for users, but worth knowing when timing requests.

### Why not call out to an OCR API?

We have a TODO-ish path here. Tesseract is free, local, and offline-friendly, but it's also the dominant source of native-deps pain in the Docker image. Alternatives if it ever becomes a maintenance burden:

- **Cloud OCR (Google Vision, AWS Textract, etc.)** — drop all native libs from the Dockerfile, swap `TesseractOcrService` for an HTTP-backed `IOcrService`. Costs per-page, but the rest of the architecture wouldn't change because the abstraction is already in place.
- **OpenAI vision / multimodal** — we already have an `OpenAiRecipeParser` for the text-to-recipe step ([src/Infrastructure/Ai/](../../src/Infrastructure/Ai/)), so going directly from image to recipe is plausible.

Keep Tesseract for now; revisit if the Dockerfile or memory footprint becomes a problem.
