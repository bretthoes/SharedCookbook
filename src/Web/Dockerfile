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
# ─── Stage 3: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV DEBIAN_FRONTEND=noninteractive

# 1. Install Tesseract & Leptonica system packages (and any imaging deps if needed)
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         tesseract-ocr \
         libleptonica-dev \
         libtesseract-dev \
         libpng16-16 \
         libjpeg62-turbo \
         libwebp7 \
         zlib1g \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# 2. Copy published .NET app
COPY --from=publish /app/publish .

# 3. Prepare /app/x64 for native libs
RUN mkdir -p /app/x64

# 4. Symlink exact libraries into /app/x64 under the names the wrapper expects:
RUN \
    # Link Leptonica: system lib is /usr/lib/x86_64-linux-gnu/libleptonica.so → we need libleptonica-1.82.0.so
    if [ -f "/usr/lib/x86_64-linux-gnu/libleptonica.so" ]; then \
      ln -sf /usr/lib/x86_64-linux-gnu/libleptonica.so /app/x64/libleptonica-1.82.0.so; \
    else \
      echo "Warning: /usr/lib/x86_64-linux-gnu/libleptonica.so not found"; \
    fi && \
    # Link Tesseract under the expected names:
    #  - libtesseract50.so
    if [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so.5" ]; then \
      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so; \
    elif [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so" ]; then \
      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so /app/x64/libtesseract50.so; \
    else \
      echo "Warning: libtesseract.so.5 or libtesseract.so not found"; \
    fi && \
    # Additionally link libtesseract.so.5 and libtesseract.so.5.0.3 if needed:
    if [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so.5" ]; then \
      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract.so.5; \
    fi && \
    if [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so.5.0.3" ]; then \
      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5.0.3 /app/x64/libtesseract.so.5.0.3; \
    fi

# 5. (Optional) Link tessdata if your code needs it:
RUN \
    if [ -d "/app/wwwroot/tessdata" ]; then \
      mkdir -p /usr/share/tessdata; \
      for f in /app/wwwroot/tessdata/*.traineddata; do \
         ln -sf "$f" "/usr/share/tessdata/$(basename \"$f\")"; \
      done; \
    fi

# 6. Ensure the loader searches /app/x64 first
ENV LD_LIBRARY_PATH="/app/x64:${LD_LIBRARY_PATH:-}"

# 7. Switch to non-root and run the app
USER $APP_UID
ENTRYPOINT ["dotnet", "SharedCookbook.Web.dll", "--urls", "http://*:$PORT"]
