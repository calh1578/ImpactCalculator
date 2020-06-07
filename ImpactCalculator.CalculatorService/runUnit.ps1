# Ignore the error if the container DNE
docker rm -f csunittests 2> $null
docker build -t csut -f UnitTests.Dockerfile .

# Exit code will be 0 if tests pass, non zero otherwise
docker run --name csunittests csut

# Attempt to copy the trx from the container
try {
    docker cp csunittests:/home/TestResults/CalculatorService.UnitTests.trx .
}
catch {
    Write-Host "Unable to copy trx"
}

docker rm -f csunittests 2> $null