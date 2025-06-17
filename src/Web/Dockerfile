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

# 1. Install Tesseract & Leptonica and common imaging dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         tesseract-ocr \
         libleptonica-dev \
         libtesseract-dev \
         libpng16-16 \
         libjpeg62-turbo \
         libwebp7 \
         zlib1g \
         # add others if ldd shows missing dependencies
    && rm -rf /var/lib/apt/lists/*

# 2. Diagnostic: list installed Leptonica/Tesseract libs and ldd output
RUN echo "=== ls for libleptonica and libtesseract ===" \
    && ls -l /usr/lib/x86_64-linux-gnu | grep -E 'libleptonica|libtesseract' || echo "None found in lib dir" \
    && echo "=== ldconfig -p for libleptonica ===" \
    && ldconfig -p | grep libleptonica || echo "No libleptonica in ldconfig" \
    && echo "=== ldconfig -p for tesseract ===" \
    && ldconfig -p | grep tesseract || echo "No libtesseract in ldconfig"

WORKDIR /app
# 3. Copy published .NET app
COPY --from=publish /app/publish .

# 4. Prepare /app/x64 for symlinks
RUN mkdir -p /app/x64

# 5. Dynamically detect and symlink Leptonica
RUN \
    LIB_LEP_SYS=$(ls /usr/lib/x86_64-linux-gnu/libleptonica.so.* 2>/dev/null | head -n1) && \
    if [ -n "$LIB_LEP_SYS" ]; then \
       echo "Linking Leptonica from $LIB_LEP_SYS"; \
       ln -sf "$LIB_LEP_SYS" /app/x64/libleptonica-1.82.0.so; \
    elif [ -f "/usr/lib/x86_64-linux-gnu/libleptonica.so" ]; then \
       echo "Linking Leptonica from unversioned"; \
       ln -sf /usr/lib/x86_64-linux-gnu/libleptonica.so /app/x64/libleptonica-1.82.0.so; \
    else \
       echo "Warning: libleptonica source .so not found"; \
    fi

# 6. Dynamically detect and symlink Tesseract
RUN \
    LIB_TESS_SYS=$(ls /usr/lib/x86_64-linux-gnu/libtesseract.so.* 2>/dev/null | head -n1) && \
    if [ -n "$LIB_TESS_SYS" ]; then \
       echo "Linking Tesseract from $LIB_TESS_SYS"; \
       # adjust target name to what wrapper expects, e.g., libtesseract50.so or libtesseract41.so
       ln -sf "$LIB_TESS_SYS" /app/x64/libtesseract50.so; \
    elif [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so" ]; then \
       echo "Linking Tesseract from unversioned"; \
       ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so /app/x64/libtesseract50.so; \
    else \
       echo "Warning: libtesseract source .so not found"; \
    fi

# 7. (Optional) Link tessdata if needed. If your code expects tessdata in /usr/share/tessdata:
RUN \
    if [ -d "/app/wwwroot/tessdata" ]; then \
      echo "Linking tessdata files..."; \
      mkdir -p /usr/share/tessdata; \
      for f in /app/wwwroot/tessdata/*.traineddata; do \
         ln -sf "$f" "/usr/share/tessdata/$(basename "$f")"; \
      done; \
    fi

# 8. Ensure loader searches /app/x64 first
ENV LD_LIBRARY_PATH="/app/x64:${LD_LIBRARY_PATH:-}"

# 9. Switch to non-root and run app
USER $APP_UID
ENTRYPOINT ["dotnet", "SharedCookbook.Web.dll", "--urls", "http://*:$PORT"]
