using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System;
using System.Linq;
using System.Net.Http;

namespace ImpactCalculator.Web.Pages
{
    public partial class Index
    {
        private string vectorString = "";
        private double score = 0;
        private string severity = "";

        private string attackComplexity = "0";
        private string attackVector = "0";
        private string privilegesRequired = "0";
        private string userInteraction = "0";
        private string scope = "0";
        private string confidentialityImpact = "0";
        private string integrityImpact = "0";
        private string availabilityImpact = "0";

        public async void CalculateAsync()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
            var options = new GrpcChannelOptions() { HttpClient = new HttpClient(handler) };

            var channel = GrpcChannel.ForAddress("http://localhost:50000", options);
            var client = new Calculator.CalculatorClient(channel);

            var vectors = new Vectors()
            {
                AttackComplexity = Enum.Parse<AttackComplexity>(attackComplexity),
                AttackVector = Enum.Parse<AttackVector>(attackVector),
                PrivilegesRequired = Enum.Parse<PrivilegesRequired>(privilegesRequired),
                UserInteraction = Enum.Parse<UserInteraction>(userInteraction),
                Scope = Enum.Parse<Scope>(scope),
                ConfidentialityImpact = Enum.Parse<Impact>(confidentialityImpact),
                IntegrityImpact = Enum.Parse<Impact>(integrityImpact),
                AvailabilityImpact = Enum.Parse<Impact>(availabilityImpact),
            };

            var cvssScore = await client.GetScoreAsync(vectors);

            score = new Random().NextDouble() * 10;

            if (score == 0)
                severity = "None";
            else if (score > 0 && score < 4)
                severity = "Low";
            else if (score > 4 && score < 7)
                severity = "Medium";
            else if (score > 7 && score < 9)
                severity = "High";
            else
                severity = "Critical";

            vectorString = "";
            vectorString += "CVSS:3.1";
            vectorString += $"/AV:{attackVector.ToString().First()}";
            vectorString += $"/AC:{attackComplexity.ToString().First()}";
            vectorString += $"/PR:{privilegesRequired.ToString().First()}";
            vectorString += $"/UI:{userInteraction.ToString().First()}";
            vectorString += $"/S:{scope.ToString().First()}";
            vectorString += $"/C:{confidentialityImpact.ToString().First()}";
            vectorString += $"/I:{integrityImpact.ToString().First()}";
            vectorString += $"/A:{availabilityImpact.ToString().First()}";
        }
    }
}
