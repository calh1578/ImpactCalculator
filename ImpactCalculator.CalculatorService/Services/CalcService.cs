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
            var vectorString = request.VectorString;
            var score = calculator.GetScore(vectorString);
            return Task.FromResult(new CvssScore() { Score = score });
        }
    }
}