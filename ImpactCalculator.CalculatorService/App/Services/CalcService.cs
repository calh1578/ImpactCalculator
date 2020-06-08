using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace ImpactCalculator.CalculatorService
{
    public class CalcService : Calculator.CalculatorBase
    {
        private readonly ILogger<CalcService> logger;
        private readonly IDefaultCvssCalculator defaultCvssCalculator;

        public CalcService(ILogger<CalcService> logger, IDefaultCvssCalculator defaultCvssCalculator)
        {
            this.logger = logger;
            this.defaultCvssCalculator = defaultCvssCalculator;
        }

        public override Task<CvssScore> GetScore(Vectors request, ServerCallContext context)
        {
            if (request == null || context == null)
            {
                logger.LogError("Null vector or context");

                return Task.FromResult(new CvssScore { Score = -1 });
            }

            var score = defaultCvssCalculator.GetScore(request.VectorString);

            return Task.FromResult(new CvssScore() { Score = score });
        }
    }
}