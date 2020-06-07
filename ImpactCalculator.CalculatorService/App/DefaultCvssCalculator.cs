using Cvss.Net;

namespace ImpactCalculator.CalculatorService
{
    public class DefaultCvssCalculator
    {
        public double GetScore(string vectorString)
        {
            if (string.IsNullOrWhiteSpace(vectorString))
            {
                return -1;
            }

            var cvss = new CvssV3(vectorString);
            var score = cvss.BaseScore;

            return score;
        }
    }
}