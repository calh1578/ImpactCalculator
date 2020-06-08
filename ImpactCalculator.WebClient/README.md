# Web Client
A web client which has a series of options user options inorder to get a Common Vulnerability Scoring System (CVSS) score.

## Details

    <PackageReference Include="Google.Protobuf" Version="3.12.1" />
    <PackageReference Include="Grpc.AspNetCore" Version="2.29.0-pre1" />
    <PackageReference Include="Grpc.Core" Version="2.29.0" />
    <PackageReference Include="Grpc.Tools" Version="2.29.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.10.9" />
    <PackageReference Include="Serilog.AspNetCore" Version="3.2.0" />

- Language : C#
- nuGet Packages :  
  - Google and Grpc* - Provides gRPC support
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets - Provides VS Container support and tools
  - Serilog.AspNetCore - Logging Provider
- Layout
  - App - Where Service code lives
  - UnitTests - Where Service code lives
  - Dockerfiles - App and Unit Tests

## Deployment
Service can be ran individually or as part of collection of services with docker-compose.

docker-compose lives in the base of ImpactCalculator Directory