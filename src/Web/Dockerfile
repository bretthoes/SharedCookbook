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

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         tesseract-ocr libleptonica-dev libtesseract-dev \
         libpng16-16 libjpeg62-turbo libtiff5 libwebp7 zlib1g \
    && rm -rf /var/lib/apt/lists/* \
    && ls -l /usr/lib/x86_64-linux-gnu | grep libleptonica || true \
    && ls -l /usr/lib/x86_64-linux-gnu | grep libtesseract || true

WORKDIR /app
COPY --from=publish /app/publish .

# Symlink native libs into /app/x64 with expected names
RUN mkdir -p /app/x64 && \
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
    fi && \
    if [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so.5" ]; then \
      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so; \
    elif [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so" ]; then \
      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so /app/x64/libtesseract50.so; \
    else \
      echo "Warning: libtesseract source .so not found"; \
    fi

ENV LD_LIBRARY_PATH="/app/x64:${LD_LIBRARY_PATH:-}"

USER $APP_UID
ENTRYPOINT ["dotnet", "SharedCookbook.Web.dll", "--urls", "http://*:$PORT"]
