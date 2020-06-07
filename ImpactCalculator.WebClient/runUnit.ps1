# Ignore the error if the container DNE
docker rm -f wcunittests 2> $null
docker build -t wcut -f UnitTests.Dockerfile .

# Exit code will be 0 if tests pass, non zero otherwise
docker run --name wcunittests csut

# Attempt to copy the trx from the container
try {
    docker cp wcunittests:/home/TestResults/WebClient.UnitTests.trx .
}
catch {
    Write-Host "Unable to copy trx"
}

docker rm -f wcunittests 2> $null