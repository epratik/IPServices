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
using Confluent.Kafka;
using StackExchange.Redis;

namespace TestIPServices
{
    [TestClass]
    public class TestController
    {
        [TestMethod]
        public async Task TestPingServiceGetAsync()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            var mockProducer = new Mock<IProducer<Null, string>>();
            var mockMultiplexer = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();

            mockConfiguration.SetupGet(m => m[It.Is<string>(s => s == "defaultServices")]).Returns("ping");
            mockConfiguration.SetupGet(m => m[It.Is<string>(s => s == "RetryCount")]).Returns("3");

            var gateway = new IPServicesGateway(mockConfiguration.Object, mockProducer.Object, mockMultiplexer.Object);
            mockProducer.Setup(m => m.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>()))
                .Returns<string, Message<Null, string>>((a, b) => Task.FromResult(new DeliveryResult<Null, string>()));

            mockMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(() => mockDatabase.Object);

            //Nothing in cache
            mockDatabase.Setup(m => m.HashKeys(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(new RedisValue[] { });

            //Found 1 record in cache
            mockDatabase.Setup(m => m.HashLength(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(1);

            //Return the record
            mockDatabase.Setup(m => m.HashGetAll(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(new HashEntry[] { new HashEntry("ping", "fetched from cache") });

            var controller = new IPController(gateway);
            var result = await controller.GetAsync(new ServiceInput() { IpAddress = "127.0.0.1", Services = "ping" });

            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);

            var model = okObjectResult.Value as HashEntry[];
            Assert.IsNotNull(model);

            var actual = model[0].Name;
            Assert.AreEqual((RedisValue)"ping", actual);
        }
    }
}
