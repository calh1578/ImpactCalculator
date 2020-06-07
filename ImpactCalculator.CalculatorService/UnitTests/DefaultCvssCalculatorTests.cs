//
// © Copyright 2020 MC
// 

using ImpactCalculator.CalculatorService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorService.UnitTests
{
    [TestClass]
    public class DefaultCvssCalculatorTests
    {
        [TestMethod]
        public void TestDefaultCvssCalculatorVectorNullExpect()
        {
            var calc = new DefaultCvssCalculator();

            Assert.AreEqual(-1, calc.GetScore(null));
        }

        [TestMethod]
        public void TestDefaultCvssCalculatorVectorEmptyExpect()
        {
            var calc = new DefaultCvssCalculator();

            calc.GetScore(string.Empty);

            Assert.AreEqual(-1, calc.GetScore(null));
        }

        [TestMethod]
        public void TestDefaultCvssCalculatorVectorValidExpectScore()
        {
            var calc = new DefaultCvssCalculator();

            Assert.AreEqual(0, calc.GetScore("CVSS:3.0/AV:P/AC:L/PR:N/UI:R/S:U/C:N/I:N/A:N"));
        }
    }
}
