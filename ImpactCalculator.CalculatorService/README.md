# Calculator Service
A service which calculates a vulnerability score based on Common Vulnerability Scoring System (CVSS) v3 Vector and sends it back to the client via gRPC.

## Details

- Language : C#
- nuGet Packages :
  - Cvss.Net - Provides CVSS scoring
  - Grpc.AspNetCore - Provides gRPC
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets - Provides VS Container support and tools
  - Serilog.AspNetCore - Logging Provider
- Layout
  - App - Where Service code lives
  - UnitTests - Where Service code lives
  - Dockerfiles - App and Unit Tests

## Deployment
Service can be ran individually or as part of collection of services with docker-compose.

docker-compose lives in the base of ImpactCalculator Directory