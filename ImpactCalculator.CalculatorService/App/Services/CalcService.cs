using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace ImpactCalculator.CalculatorService
{
    public class CalcService : Calculator.CalculatorBase
    {
        private readonly DefaultCvssCalculator calculator = new DefaultCvssCalculator();
        private readonly ILogger<CalcService> _logger;

        public CalcService(ILogger<CalcService> logger)
        {
            _logger = logger;
        }

        public override Task<CvssScore> GetScore(Vectors request, ServerCallContext context)
        {
            if (request == null || context == null)
            {
                _logger.LogError("Null vector or context");

                return Task.FromResult(new CvssScore { Score = -1 });
            }

            var vectorString = request.VectorString;

            double score;

            try
            {
                score = calculator.GetScore(vectorString);
            }
            catch
            {
                _logger.LogError($"Invalid vector string, vector:{request.VectorString}");

                score = -1;
            }

            return Task.FromResult(new CvssScore() { Score = score });
        }
    }
}