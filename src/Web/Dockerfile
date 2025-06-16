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
    && apt-get install -y --no-install-recommends tesseract-ocr libleptonica-dev libtesseract-dev \
    \
    # Debug: list exact files and ldconfig entries
    && echo "---- Files under /usr/lib/x86_64-linux-gnu for libleptonica ----" \
    && ls -l /usr/lib/x86_64-linux-gnu | grep libleptonica || echo "None found" \
    && echo "---- ldconfig -p for libleptonica ----" \
    && ldconfig -p | grep libleptonica || echo "None found in ldconfig" \
    \
    # Symlink logic
    && LIB_LEP=$(ls /usr/lib/x86_64-linux-gnu/libleptonica.so.* 2>/dev/null | head -n1) \
    && if [ -z "$LIB_LEP" ]; then LIB_LEP="/usr/lib/x86_64-linux-gnu/libleptonica.so"; fi \
    && ln -sf "$LIB_LEP" /usr/lib/x86_64-linux-gnu/libleptonica-1.82.0.so \
    \
    # Similarly for tesseract, if needed
    && LIB_TESS=$(ls /usr/lib/x86_64-linux-gnu/libtesseract.so.* 2>/dev/null | head -n1) \
    && if [ -z "$LIB_TESS" ]; then LIB_TESS="/usr/lib/x86_64-linux-gnu/libtesseract.so"; fi \
    && ln -sf "$LIB_TESS" /usr/lib/x86_64-linux-gnu/libtesseract.so \
    \
    && echo "---- After symlink: ls libleptonica-1.82.0.so ----" \
    && ls -l /usr/lib/x86_64-linux-gnu/libleptonica-1.82.0.so || echo "Symlink missing" \
    && echo "---- After symlink: ldconfig -p for libleptonica ----" \
    && ldconfig -p | grep libleptonica || echo "Still not in ldconfig" \
    \
    && ldconfig \
    && rm -rf /var/lib/apt/lists/* 


    #ln -s /usr/lib/libleptonica.so.6.0.0 /app/x64/libleptonica-1.82.0.so
    #ln -s /usr/lib/libtesseract.so.5.0.3 /app/x64/libtesseract50.so
    #ln -s /app/tessdata/eng.traineddata /usr/share/tessdata/eng.traineddata



WORKDIR /app
COPY --from=publish /app/publish .

# Tell the dynamic loader to look in /app first
ENV LD_LIBRARY_PATH="/app:${LD_LIBRARY_PATH:-}"
USER $APP_UID

ENTRYPOINT ["sh", "-c", "dotnet SharedCookbook.Web.dll --urls http://*:$PORT"]
