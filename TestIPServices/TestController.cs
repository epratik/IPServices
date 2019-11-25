using IPServiceAggregator;
using IPServiceAggregator.Core;
using IPServiceAggregator.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IPServiceAggregator.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestIPServices
{
    [TestClass]
    public class TestController
    {
        [TestMethod]
        public async Task TestPingServiceGetAsync()
        {
            var mockLogger = new Mock<ILogger<PingService>>();
            var mockFactory = new Mock<IIPServicesFactory>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.SetupGet(m => m[It.Is<string>(s => s == "defaultServices")]).Returns("ping");
            mockFactory.Setup(m => m.GetInstance("ping")).Returns(new PingService(mockLogger.Object));

            var gateway = new IPServicesGateway(mockConfiguration.Object, mockFactory.Object);
            
            var controller = new IPController(gateway);
            var result = await controller.GetAsync(new ServiceInput() { IpAddress = "127.0.0.1", Services = "ping" });
           
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);

            var model = okObjectResult.Value as Result[];
            Assert.IsNotNull(model);

            var actual = model[0].Service;
            Assert.AreEqual("PING", actual);
        }
    }
}
