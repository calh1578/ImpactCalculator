//
// © Copyright 2020 MC
//

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Grpc.Core;
using ImpactCalculator.WebClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WebClient.UnitTests
{
    [TestClass]
    public class IndexPageTests
    {
        private Mock<ILogger<ImpactCalculator.WebClient.Pages.Index>> loggerMock = new Mock<ILogger<ImpactCalculator.WebClient.Pages.Index>>();
        private Mock<IDistributedCache> cacheMock = new Mock<IDistributedCache>();

        [DataTestMethod]
        [DataRow(0, "None")]
        [DataRow(2.5, "Low")]
        [DataRow(6, "Medium")]
        [DataRow(7, "Medium")]
        [DataRow(8, "High")]
        [DataRow(9, "High")]
        [DataRow(9.9, "Critical")]
        public async Task TestIndexPageWithVariousScoresNoCacheExpectSuccess(double score, string severity)
        {
            var ctx = new Bunit.TestContext();
            cacheMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            // Inject
            ctx.Services.Add(new ServiceDescriptor(typeof(Calculator.CalculatorClient), new CalculatorClientStub(true, score)));
            ctx.Services.Add(new ServiceDescriptor(typeof(ILogger<ImpactCalculator.WebClient.Pages.Index>), loggerMock.Object));
            ctx.Services.Add(new ServiceDescriptor(typeof(IDistributedCache), cacheMock.Object));

            var indexPage = ctx.RenderComponent<ImpactCalculator.WebClient.Pages.Index>();

            // Press the button on page (In Memory)
            await indexPage.Find("button").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            // Grab the labels, iterate over them, verify data
            var labels = indexPage.FindAll("label");

            foreach (var label in labels)
            {
                if (label.Id == "severity")
                {
                    Assert.AreEqual(severity, label.InnerHtml);
                }
            }
        }

        [TestMethod]
        public async Task TestIndexPageWithCacheExpectSuccessSeverityMatch()
        {
            var vectorString = "CVSS:3.0/AV:P/AC:L/PR:N/UI:R/S:U/C:N/I:N/A:N";
            var score = new CvssScore { Score = 5 };

            var scoreSerialized = JsonSerializer.Serialize(score);
            var scoreSerializedBytes = Encoding.UTF8.GetBytes(scoreSerialized);

            var ctx = new Bunit.TestContext();

            byte[] bytesNull = null;

            cacheMock.SetupSequence(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).
                ReturnsAsync(bytesNull)
                .ReturnsAsync(scoreSerializedBytes);

            // Inject
            ctx.Services.Add(new ServiceDescriptor(typeof(Calculator.CalculatorClient), new CalculatorClientStub(true, 5)));
            ctx.Services.Add(new ServiceDescriptor(typeof(ILogger<ImpactCalculator.WebClient.Pages.Index>), loggerMock.Object));
            ctx.Services.Add(new ServiceDescriptor(typeof(IDistributedCache), cacheMock.Object));

            var indexPage = ctx.RenderComponent<ImpactCalculator.WebClient.Pages.Index>();

            // Press the button on page (In Memory)
            await indexPage.Find("button").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            // Grab the labels, iterate over them, verify data
            var labels = indexPage.FindAll("label");

            var firstSeverity = string.Empty;

            foreach (var label in labels)
            {
                if (label.Id == "severity")
                {
                    firstSeverity = label.InnerHtml;
                }
            }

            // Press the button again on page (In Memory) to test cache
            await indexPage.Find("button").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            var secondSeverity = string.Empty;

            foreach (var label in labels)
            {
                if (label.Id == "severity")
                {
                    secondSeverity = label.InnerHtml;
                }
            }

            Assert.AreEqual(firstSeverity, secondSeverity);

            cacheMock.Verify(mock => mock.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task TestIndexPageWithNoGrpcServerExpectNegativeScoreAndNoSeverity()
        {
            var ctx = new Bunit.TestContext();
            cacheMock.Setup(mock => mock.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            // Inject
            ctx.Services.Add(new ServiceDescriptor(typeof(Calculator.CalculatorClient), new CalculatorClientStub(false, 0)));
            ctx.Services.Add(new ServiceDescriptor(typeof(ILogger<ImpactCalculator.WebClient.Pages.Index>), loggerMock.Object));
            ctx.Services.Add(new ServiceDescriptor(typeof(IDistributedCache), cacheMock.Object));

            var indexPage = ctx.RenderComponent<ImpactCalculator.WebClient.Pages.Index>();

            // Press the button on page (In Memory)
            await indexPage.Find("button").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            var labels = indexPage.FindAll("label");

            // Grab the labels, iterate over them, verify data
            foreach (var label in labels)
            {
                if (label.Id == "score")
                {
                    Assert.AreEqual("-1", label.InnerHtml);
                }

                if (label.Id == "severity")
                {
                    Assert.AreEqual("Unable to obtain severity", label.InnerHtml);
                }
            }
        }

        /// <summary>
        /// A gRPC Stub to allow overriding of GetScoreAsync
        /// </summary>
        public class CalculatorClientStub : Calculator.CalculatorClient
        {
            private bool ReturnData { get; set; }

            private double Score { get; set; }

            public CalculatorClientStub(bool returnData, double score)
            {
                ReturnData = returnData;
                Score = score;
            }

            public override AsyncUnaryCall<CvssScore> GetScoreAsync(Vectors request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            {
                if (ReturnData)
                {
                    return new AsyncUnaryCall<CvssScore>(Task.FromResult(new CvssScore { Score = Score }), null, null, null, null);
                }

                return null;
            }
        }
    }
}