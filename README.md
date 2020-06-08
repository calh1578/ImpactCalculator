# Impact Calculator
Impact Calculator is a basic Client/Server application, which utilizes gRPC and Common Vulnerability Scoring System (CVSS) to tell the user their vulnerability severity based on a series of questions.

## ImpactCalculator.CalculatorService
The Server written in C# and uses gRPC

## ImpactCalculator.WebClient
The Client written in C#, uses Blazor, and gRPC.

## Deployment
Each service each can be deployed multiple ways:
 - Hosted individually by VS
 - Hosted individually by VS via Docker
 - Hosted together by VS via docker-compose
 - Hosted together via docker-compose

### docker-compose
To bring deploy both services via docker-compose, perform the following steps

0. Docker Desktop is installed and is set to Linux containers
1. Navigate to the Base Directory of ImpactCalculator using PowerShell
2. Execute the following command `docker-compose build`
   1. This will build the services listed in the `docker-compose.yml` file
3. Execute the following command `docker-compose up -d --no-build --remove-orphans`
   1. This will start the services 
4. Verify services are running by executing the following command `docker ps`
5. Open your favorite browser and navigate to [Here](http://localhost:8000)
6. Enter some values, get some scores and severity
7. Once done, cleanup by running `docker-compose down`

## Logging
Logging is being handled by Serilog, which has many configuration options.  Log levels for the service and AspNet can be set via the docker-compose
or with environment levels.

## Testing
Each Service has its own Unit Test Suite.  The suite can be ran multiple ways
 - From within Visual Studio
 - From the command line inside the UnitTest folder with `dotnet test`
 - Running inside of Docker
   - Inside each Service directory, there is a `runUnit.ps1` which creates an image, runs a container from the image, copies the trx out of the container, deletes the container