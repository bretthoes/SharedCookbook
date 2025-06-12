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
         tesseract-ocr \
         libleptonica-dev \
         libtesseract-dev \
    \
    # Create unversioned symlinks for P/Invoke
    && for lib in /usr/lib/x86_64-linux-gnu/libtesseract.so.*; do \
         ln -sf "$lib" /usr/lib/x86_64-linux-gnu/libtesseract.so; \
         break; \
       done \
    && for lib in /usr/lib/x86_64-linux-gnu/libleptonica.so.*; do \
         ln -sf "$lib" /usr/lib/x86_64-linux-gnu/libleptonica.so; \
         break; \
       done \
    && ldconfig \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "dotnet SharedCookbook.Web.dll --urls http://*:$PORT"]
