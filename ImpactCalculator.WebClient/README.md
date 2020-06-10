# Web Client
A web client which has a series of options user options inorder to get a Common Vulnerability Scoring System (CVSS) score.

## Details

- Language : C#
- nuGet Packages :  
  - Google and Grpc* - Provides gRPC support
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets - Provides VS Container support and tools
  - Serilog.AspNetCore - Logging Provider
  - Redis* - Support for Redis
  - System.Text.Json - Used for Serial-Deserial
- Layout
  - App - Where Service code lives
  - UnitTests - Where Service code lives
  - Dockerfiles - App and Unit Tests
- Redis
  - Support for using Redis with docker-compose
  - Will not use cache if cache not available

## Deployment
Service can be ran individually or as part of collection of services with docker-compose.

docker-compose lives in the base of ImpactCalculator Directory