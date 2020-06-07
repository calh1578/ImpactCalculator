FROM mcr.microsoft.com/dotnet/core/sdk:3.1

ADD App/ImpactCalculator.WebClient.csproj App/
ADD UnitTests/WebClient.UnitTests.csproj UnitTests
RUN dotnet restore UnitTests

ADD App App
WORKDIR UnitTests
ADD UnitTests .

RUN dotnet build -c Release

CMD ["dotnet", "test", "--no-restore", "--no-build", "-c", "Release", "--logger", "trx;LogFileName=//home//TestResults//WebClient.UnitTests.trx"]