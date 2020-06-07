FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

#ENV ASPNETCORE_URLS=https://+:443

WORKDIR /app

COPY App/ImpactCalculator.CalculatorService.csproj .

RUN dotnet restore

COPY App/. .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/sdk:3.1

WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "ImpactCalculator.CalculatorService.dll"]