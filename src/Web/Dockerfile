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

# 1. Install Tesseract & Leptonica system packages (and common imaging deps if needed)
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         tesseract-ocr \
         libleptonica-dev \
         libtesseract-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
# 2. Copy published .NET app
COPY --from=publish /app/publish .

# 3. Prepare /app/x64 for native libs
# RUN mkdir -p /app/x64

# 4. Symlink or copy exact libraries into /app/x64 under the names you want:
#RUN \
#    # libleptonica-1.8.2.so: link from the unversioned lib (since that's what exists)
#    if [ -f "/usr/lib/x86_64-linux-gnu/libleptonica.so" ]; then \
#      ln -sf /usr/lib/x86_64-linux-gnu/libleptonica.so /app/x64/libleptonica-1.8.2.so; \
#    else \
#      echo "Warning: /usr/lib/x86_64-linux-gnu/libleptonica.so not found"; \
#    fi && \
#    # libtesseract50.so: link from libtesseract.so.5 (or fallback)
#    if [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so.5" ]; then \
#      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so; \
#    elif [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so" ]; then \
#      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so /app/x64/libtesseract50.so; \
#    else \
#      echo "Warning: libtesseract.so.5 or libtesseract.so not found"; \
#    fi && \
#    # libtesseract.so.5: link explicitly as well (though libtesseract.so → so.5.0.3 already)
#    if [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so.5" ]; then \
#      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract.so.5; \
#    else \
#      echo "Warning: /usr/lib/x86_64-linux-gnu/libtesseract.so.5 not found"; \
#    fi && \
#    # libtesseract.so.5.0.3: link explicitly
#    if [ -f "/usr/lib/x86_64-linux-gnu/libtesseract.so.5.0.3" ]; then \
#      ln -sf /usr/lib/x86_64-linux-gnu/libtesseract.so.5.0.3 /app/x64/libtesseract.so.5.0.3; \
#    else \
#      echo "Warning: /usr/lib/x86_64-linux-gnu/libtesseract.so.5.0.3 not found"; \
#    fi

# 6. Ensure the dynamic loader searches /app/x64 first
# ENV LD_LIBRARY_PATH="/app/x64:${LD_LIBRARY_PATH:-}"

# 7. Switch to non-root user and run the app
USER $APP_UID
ENTRYPOINT ["dotnet", "SharedCookbook.Web.dll", "--urls", "http://*:$PORT"]
