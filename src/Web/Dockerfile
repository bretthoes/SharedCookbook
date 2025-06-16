# ─── Stage 1: Build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY *.sln                     ./
COPY Directory.Packages.props  ./
COPY Directory.Build.props     ./
COPY global.json               ./

COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Web/Web.csproj"

COPY . .
WORKDIR "/src/src/Web"

# Build without running NSwag
RUN dotnet build "Web.csproj" -c $BUILD_CONFIGURATION -p:SkipNSwag=true

# ─── Stage 2: Publish ──────────────────────────────────────────────────────────
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/Web"

RUN dotnet publish "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    -p:SkipNSwag=true \
    /p:UseAppHost=false

# ─── Stage 3: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV DEBIAN_FRONTEND=noninteractive

# 1. Install Tesseract & Leptonica system packages
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         tesseract-ocr \
         libleptonica-dev \
         libtesseract-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
# 2. Copy published .NET app
COPY --from=publish /app/publish .

# 3. Create a folder for native symlinks, adjust ownership if needed
RUN mkdir -p /app/x64

# 4. Symlink real system libs into /app/x64 under the names expected by P/Invoke.
#    Use the actual versioned filenames you saw in your diagnostics.
#    For example, if you saw /usr/lib/x86_64-linux-gnu/libleptonica.so.6.0.0, use that.
#    Here’s a pattern: find the real file, then symlink to the expected name.
RUN \
    # Find actual libleptonica file (adjust the pattern if needed):
    LIB_LEP_SYS=$(ls /usr/lib/x86_64-linux-gnu/libleptonica.so.* 2>/dev/null | head -n1) \
    && if [ -z "$LIB_LEP_SYS" ]; then LIB_LEP_SYS="/usr/lib/x86_64-linux-gnu/libleptonica.so"; fi \
    && ln -sf "$LIB_LEP_SYS" /app/x64/libleptonica-1.82.0.so \
    \
    # Find actual tesseract library (adjust if your diagnostics showed a versioned name):
    && LIB_TESS_SYS=$(ls /usr/lib/x86_64-linux-gnu/libtesseract.so.* 2>/dev/null | head -n1) \
    && if [ -z "$LIB_TESS_SYS" ]; then LIB_TESS_SYS="/usr/lib/x86_64-linux-gnu/libtesseract.so"; fi \
    && ln -sf "$LIB_TESS_SYS" /app/x64/libtesseract50.so

# 5. (Optional) If you have tessdata in your published app (e.g. under wwwroot/tessdata),
#    and your code or TesseractEngine expects to find it under /usr/share/tessdata, you can symlink:
#    Adapt paths: if your publish includes wwwroot/tessdata/eng.traineddata, link it into system tessdata folder:
RUN \
    if [ -d "/app/wwwroot/tessdata" ]; then \
      # Ensure system tessdata dir exists:
      mkdir -p /usr/share/tessdata; \
      # Link each traineddata file:
      for f in /app/wwwroot/tessdata/*.traineddata; do \
        ln -sf "$f" "/usr/share/tessdata/$(basename "$f")"; \
      done; \
    fi

# 6. Ensure the dynamic loader searches /app/x64 first (and optionally /app)
ENV LD_LIBRARY_PATH="/app/x64:/app:${LD_LIBRARY_PATH:-}"

# 7. Switch to non-root user if required by your app
USER $APP_UID

ENTRYPOINT ["dotnet", "SharedCookbook.Web.dll", "--urls", "http://*:$PORT"]
