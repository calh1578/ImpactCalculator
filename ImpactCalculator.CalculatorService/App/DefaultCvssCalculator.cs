using System;
using Cvss.Net;
using Microsoft.Extensions.Logging;

namespace ImpactCalculator.CalculatorService
{
    public interface IDefaultCvssCalculator
    {
        double GetScore(string vectorString);
    }

    public class DefaultCvssCalculator : IDefaultCvssCalculator
    {
        private readonly ILogger<DefaultCvssCalculator> logger;

        public DefaultCvssCalculator(ILogger<DefaultCvssCalculator> logger)
        {
            this.logger = logger;
        }

        public double GetScore(string vectorString)
        {
            if (string.IsNullOrWhiteSpace(vectorString))
            {
                logger.LogWarning("Vector String was null or empty");

                return -1;
            }

            CvssV3 cvss;

            try
            {
                cvss = new CvssV3(vectorString);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception due to vector string: {vectorString}");

                return -1;
            }

            var score = cvss.BaseScore;

            return score;
        }
    }
}