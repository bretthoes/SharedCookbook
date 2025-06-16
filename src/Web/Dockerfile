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

# 3. Create directory for native symlinks next to your app binaries
RUN mkdir -p /app/x64

# 4. Create symlinks for Leptonica and Tesseract with the expected versioned names.
#    Adjust source filenames (/usr/lib/x86_64-linux-gnu/...) to those actually present.
#    E.g., if apt installed /usr/lib/x86_64-linux-gnu/libleptonica.so.5, link it to libleptonica-1.78.0.so
#    If it installed /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1, link it to libtesseract41.so.
# Symlink for Leptonica:
RUN \
    mkdir -p /app/x64 && \
    if [ -f "/usr/lib/x86_64-linux-gnu/libleptonica.so.5" ]; then \
       ln -sf /usr/lib/x86_64-linux-gnu/libleptonica.so.5 /app/x64/libleptonica-1.82.0.so; \
    elif [ -f "/usr/lib/x86_64-linux-gnu/libleptonica.so" ]; then \
       ln -sf /usr/lib/x86_64-linux-gnu/libleptonica.so /app/x64/libleptonica-1.82.0.so; \
    elif [ -f "/usr/lib/x86_64-linux-gnu/liblept.so.5" ]; then \
           ln -sf /usr/lib/x86_64-linux-gnu/liblept.so.5 /app/x64/libleptonica-1.82.0.so; \
    elif [ -f "/usr/lib/x86_64-linux-gnu/liblept.so" ]; then \
               ln -sf /usr/lib/x86_64-linux-gnu/liblept.so /app/x64/libleptonica-1.82.0.so; \
    else \
       echo "Warning: libleptonica source .so not found"; \
    fi


# 5. If your code expects tessdata under a particular path (e.g. alongside app), you can also link traineddata files:
#    Suppose you have wwwroot/tessdata/*.traineddata in your publish output:
RUN \
    if [ -d "/app/wwwroot/tessdata" ]; then \
      mkdir -p /usr/share/tessdata; \
      for f in /app/wwwroot/tessdata/*.traineddata; do \
        ln -sf "$f" "/usr/share/tessdata/$(basename "$f")"; \
      done; \
    fi

# 6. Ensure the loader searches /app/x64 first
ENV LD_LIBRARY_PATH="/app/x64:${LD_LIBRARY_PATH:-}"

# 7. Switch to non-root if needed
USER $APP_UID

ENTRYPOINT ["dotnet", "SharedCookbook.Web.dll", "--urls", "http://*:$PORT"]
