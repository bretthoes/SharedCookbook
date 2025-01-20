# SharedCookbook

## About

API for the [cookbook-mobile app](https://github.com/bretthoes/cookbook-mobile). It adheres to the OpenAPI specification.

## Build

Run `dotnet build -tl` to build the solution.

## Run

To run the API locally, run:

```bash
cd .\src\Web\
dotnet watch run
```

Navigate to https://localhost:5001. The application will automatically reload if you change any of the source files.

Can ping API at `https://localhost:5001/health` as well.

## Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

## Acknowledgement

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/SharedCookbook) version 8.0.5.

