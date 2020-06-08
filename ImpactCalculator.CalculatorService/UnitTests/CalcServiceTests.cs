//
// © Copyright 2020 MC
// 

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using ImpactCalculator.CalculatorService;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CalculatorService.UnitTests
{
    [TestClass]
    public class CalcServiceTests
    {
        private readonly Mock<ILogger<CalcService>> mockLogger = new Mock<ILogger<CalcService>>();
        private readonly Mock<IDefaultCvssCalculator> mockCalculator = new Mock<IDefaultCvssCalculator>();

        [TestMethod]
        public async Task TestCalcServiceVectorNullContextNullExpectNegativeScore()
        {
            var calcService = new CalcService(mockLogger.Object, mockCalculator.Object);

            var score = await calcService.GetScore(new Vectors(), null);

            Assert.AreEqual(-1, score.Score);

            score = await calcService.GetScore(null, new ServerCallContextInherit());

            Assert.AreEqual(-1, score.Score);
        }

        [DataTestMethod]
        [DataRow("CVSS:3.0/AV:P/AC:L/PR:N/UI:R/S:U/C:N/I:N/A:N", 0)]
        [DataRow("CVSS:3.0/AV:L/AC:H/PR:L/UI:N/S:C/C:L/I:L/A:L", 5.3)]
        [DataRow("CVSS:3.0/AV:A/AC:L/PR:H/UI:R/S:U/C:H/I:H/A:H", 6.6)]
        [DataRow("CVSS:3.0/AV:N/AC:H/PR:N/UI:N/S:C/C:N/I:N/A:L", 4)]
        [DataRow("CVSS:3.0/AV:P/AC:L/PR:L/UI:R/S:U/C:L/I:L/A:L", 3.9)]
        [DataRow("CVSS:3.0/AV:P/AC:L/PR:L/UI:R/S:U/C:L/I:L/A:K", -1)]
        public async Task TestCalcServiceVectorValidContextValidExpectCorrectScore(string vector, double correctScore)
        {
            mockCalculator.Setup(mock => mock.GetScore(vector)).Returns(correctScore);

            var calcService = new CalcService(mockLogger.Object, mockCalculator.Object);

            var score = await calcService.GetScore(new Vectors { VectorString = vector }, new ServerCallContextInherit());

            Assert.AreEqual(correctScore, score.Score);
        }
    }

    public class ServerCallContextInherit : ServerCallContext
    {
        protected override AuthContext AuthContextCore => throw new NotImplementedException();

        protected override CancellationToken CancellationTokenCore => throw new NotImplementedException();

        protected override DateTime DeadlineCore => throw new NotImplementedException();

        protected override string HostCore => throw new NotImplementedException();

        protected override string MethodCore => throw new NotImplementedException();

        protected override string PeerCore => throw new NotImplementedException();

        protected override Metadata RequestHeadersCore => throw new NotImplementedException();

        protected override Metadata ResponseTrailersCore => throw new NotImplementedException();

        protected override Status StatusCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override WriteOptions WriteOptionsCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
        {
            throw new NotImplementedException();
        }
    }
}