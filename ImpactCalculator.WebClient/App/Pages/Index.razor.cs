using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ImpactCalculator.WebClient.Pages
{
    public partial class Index
    {
        private string vectorString = string.Empty;
        private double score = 0;
        private string severity = string.Empty;

        private string attackComplexity = "Low";
        private string attackVector = "Physical";
        private string privilegesRequired = "None";
        private string userInteraction = "Required";
        private string scope = "Unchanged";
        private string confidentialityImpact = "None";
        private string integrityImpact = "None";
        private string availabilityImpact = "None";

        [Inject] Calculator.CalculatorClient CalculatorClient { get; set; }

        [Inject] private ILogger<Index> Logger { get; set; }

        [Inject] private IDistributedCache Cache { get; set; }

        public async Task CalculateAsync()
        {
            vectorString = string.Empty;
            vectorString += "CVSS:3.0";
            vectorString += $"/AV:{attackVector.First()}";
            vectorString += $"/AC:{attackComplexity.First()}";
            vectorString += $"/PR:{privilegesRequired.First()}";
            vectorString += $"/UI:{userInteraction.First()}";
            vectorString += $"/S:{scope.First()}";
            vectorString += $"/C:{confidentialityImpact.First()}";
            vectorString += $"/I:{integrityImpact.First()}";
            vectorString += $"/A:{availabilityImpact.First()}";

            Logger.LogInformation($"User vector string: {vectorString}");

            var vectors = new Vectors() { VectorString = vectorString };

            CvssScore cvssScore;

            try
            {
                Logger.LogDebug($"Attempting to connect to gRPC Server");

                try
                {
                    var cacheBytes = await Cache.GetAsync(vectors.VectorString);

                    if (cacheBytes == null)
                    {
                        cvssScore = await CalculatorClient.GetScoreAsync(vectors);

                        var cvssScoreSerialized = JsonSerializer.Serialize(cvssScore);
                        var cvssScoreBytes = Encoding.UTF8.GetBytes(cvssScoreSerialized);

                        Logger.LogInformation($"Add cache for vector string: {vectorString}");
                        await Cache.SetAsync(vectors.VectorString, cvssScoreBytes, new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                        });
                    }
                    else
                    {
                        Logger.LogInformation($"Used cache for vector string: {vectorString}");

                        var bytesString = Encoding.UTF8.GetString(cacheBytes);
                        cvssScore = JsonSerializer.Deserialize<CvssScore>(bytesString);
                    }
                }
                catch
                {
                    Logger.LogWarning("Unable to use cache");

                    cvssScore = await CalculatorClient.GetScoreAsync(vectors);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Unable to connect to gRPC Server");

                cvssScore = new CvssScore
                {
                    Score = -1
                };
            }

            score = cvssScore.Score;

            if (score == 0)
                severity = "None";
            else if (score > 0 && score <= 4)
                severity = "Low";
            else if (score > 4 && score <= 7)
                severity = "Medium";
            else if (score > 7 && score <= 9)
                severity = "High";
            else if (score > 9)
                severity = "Critical";
            else
                severity = "Unable to obtain severity";

            StateHasChanged();
        }
    }
}