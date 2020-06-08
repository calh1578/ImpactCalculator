//
// © Copyright 2020 MC
// 

using ImpactCalculator.CalculatorService;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CalculatorService.UnitTests
{
    [TestClass]
    public class DefaultCvssCalculatorTests
    {
        private readonly Mock<ILogger<DefaultCvssCalculator>> loggerMock = new Mock<ILogger<DefaultCvssCalculator>>();

        [TestMethod]
        public void TestDefaultCvssCalculatorVectorNullExpect()
        {
            var calc = new DefaultCvssCalculator(loggerMock.Object);

            Assert.AreEqual(-1, calc.GetScore(null));
        }

        [TestMethod]
        public void TestDefaultCvssCalculatorVectorEmptyExpect()
        {
            var calc = new DefaultCvssCalculator(loggerMock.Object);

            calc.GetScore(string.Empty);

            Assert.AreEqual(-1, calc.GetScore(null));
        }

        [TestMethod]
        public void TestDefaultCvssCalculatorVectorValidExpectScore()
        {
            var calc = new DefaultCvssCalculator(loggerMock.Object);

            Assert.AreEqual(0, calc.GetScore("CVSS:3.0/AV:P/AC:L/PR:N/UI:R/S:U/C:N/I:N/A:N"));
        }
    }
}
