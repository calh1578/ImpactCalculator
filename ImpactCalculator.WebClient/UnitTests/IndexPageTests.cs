//
// © Copyright 2020 MC
//

using System;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Grpc.Core;
using ImpactCalculator.WebClient;
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

        [DataTestMethod]
        [DataRow(0, "None")]
        [DataRow(2.5, "Low")]
        [DataRow(6, "Medium")]
        [DataRow(7, "Medium")]
        [DataRow(8, "High")]
        [DataRow(9, "High")]
        [DataRow(9.9, "Critical")]
        public async Task TestIndexPageWithVariousScoresExpectSuccess(double score, string severity)
        {
            var ctx = new Bunit.TestContext();

            // Inject
            ctx.Services.Add(new ServiceDescriptor(typeof(Calculator.CalculatorClient), new CalculatorClientStub(true, score)));
            ctx.Services.Add(new ServiceDescriptor(typeof(ILogger<ImpactCalculator.WebClient.Pages.Index>), loggerMock.Object));

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
        public async Task TestIndexPageWithNoGrpcServerExpectNegativeScoreAndNoSeverity()
        {
            var ctx = new Bunit.TestContext();

            // Inject
            ctx.Services.Add(new ServiceDescriptor(typeof(Calculator.CalculatorClient), new CalculatorClientStub(false, 0)));
            ctx.Services.Add(new ServiceDescriptor(typeof(ILogger<ImpactCalculator.WebClient.Pages.Index>), loggerMock.Object));

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