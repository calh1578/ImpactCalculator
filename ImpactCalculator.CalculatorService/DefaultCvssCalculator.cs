using Cvss.Net;

namespace ImpactCalculator.CalculatorService
{
    public class DefaultCvssCalculator
    {
        public double GetScore(string vectorString)
        {
            var cvss = new CvssV3(vectorString);
            var score = cvss.BaseScore;
            return score;
        }
    }
}