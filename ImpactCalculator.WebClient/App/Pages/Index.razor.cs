using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ImpactCalculator.WebClient.Pages
{
    public partial class Index
    {
        private string vectorString = "";
        private double score = 0;
        private string severity = "";

        private string attackComplexity = "Low";
        private string attackVector = "Physical";
        private string privilegesRequired = "None";
        private string userInteraction = "Required";
        private string scope = "Unchanged";
        private string confidentialityImpact = "None";
        private string integrityImpact = "None";
        private string availabilityImpact = "None";

        [Inject] Calculator.CalculatorClient CalculatorClient { get; set; }

        public async Task CalculateAsync()
        {
            vectorString = "";
            vectorString += "CVSS:3.0";
            vectorString += $"/AV:{attackVector.First()}";
            vectorString += $"/AC:{attackComplexity.First()}";
            vectorString += $"/PR:{privilegesRequired.First()}";
            vectorString += $"/UI:{userInteraction.First()}";
            vectorString += $"/S:{scope.First()}";
            vectorString += $"/C:{confidentialityImpact.First()}";
            vectorString += $"/I:{integrityImpact.First()}";
            vectorString += $"/A:{availabilityImpact.First()}";

            var vectors = new Vectors() { VectorString = vectorString };

            CvssScore cvssScore;

            try
            {
                cvssScore = await CalculatorClient.GetScoreAsync(vectors);
            }
            catch
            {
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