# SharedCookbook

## About

Backend API for [cookbook-mobile repo](https://github.com/bretthoes/cookbook-mobile), adheres to [OpenAPI Specification v3.1.0](https://spec.openapis.org/oas/v3.1.0.html).

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

## Technologies

- [.NET 10](https://dotnet.microsoft.com/)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core 10](https://learn.microsoft.com/ef/core)
- [FluentValidation](https://fluentvalidation.net/)
- [NUnit](https://nunit.org/)
- [Moq](https://github.com/moq/moq4)
- [Respawn](https://github.com/jbogard/Respawn)

## Architecture

- [Clean Architecture Template by Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)
- Domain-Driven Design (DDD)

## Integrations

- [Microsoft Azure](https://azure.microsoft.com/) – API & PostgreSQL hosting
- [Amazon S3 (AWS)](https://aws.amazon.com/s3/) – Image storage
- [Google Identity (OAuth)](https://developers.google.com/identity) – Single Sign-On
- [Tesseract OCR](https://github.com/tesseract-ocr/tesseract) – Recipe text extraction
- [Spoonacular API](https://spoonacular.com/food-api) – Recipe parsing
- [Mailgun](https://www.mailgun.com/) – Email delivery
- [Namecheap](https://www.namecheap.com/) – Email domain (SPF/DKIM setup)


## Acknowledgement

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/SharedCookbook) version 8.0.5.

