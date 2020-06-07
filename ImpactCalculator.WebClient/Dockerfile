FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app

COPY App/ImpactCalculator.WebClient.csproj .

RUN dotnet restore

COPY App/. .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "ImpactCalculator.WebClient.dll"]