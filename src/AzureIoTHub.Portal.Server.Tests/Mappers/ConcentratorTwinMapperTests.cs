﻿using AzureIoTHub.Portal.Server.Extensions;
using AzureIoTHub.Portal.Server.Mappers;
using AzureIoTHub.Portal.Shared.Models.Concentrator;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static AzureIoTHub.Portal.Server.Startup;

namespace AzureIoTHub.Portal.Server.Tests.Mappers
{
    [TestFixture]
    public class ConcentratorTwinMapperTests
    {
        private MockRepository mockRepository;

        private HttpClient mockHttpClient;
        private Mock<IConfiguration> mockConfiguration;
        private Mock<HttpMessageHandler> httpMessageHandlerMock;

        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();
            this.httpMessageHandlerMock = this.mockRepository.Create<HttpMessageHandler>();
            this.mockHttpClient = new HttpClient(this.httpMessageHandlerMock.Object);
        }

        private ConcentratorTwinMapper CreateConcentratorTwinMapper()
        {
            return new ConcentratorTwinMapper(mockConfiguration.Object, this.mockHttpClient);
        }

        [Test]
        public void CreateDeviceDetails_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var concentratorTwinMapper = this.CreateConcentratorTwinMapper();
            Twin twin = new Twin
            {
                DeviceId = Guid.NewGuid().ToString()
            };

            twin.Tags[nameof(Concentrator.DeviceType).ToCamelCase()] = Guid.NewGuid().ToString();
            twin.Tags[nameof(Concentrator.DeviceFriendlyName).ToCamelCase()] = Guid.NewGuid().ToString();
            twin.Tags[nameof(Concentrator.LoraRegion).ToCamelCase()] = Guid.NewGuid().ToString();

            twin.Properties.Reported["DevAddr"] = Guid.NewGuid().ToString();

            twin.Properties.Desired[nameof(Concentrator.ClientCertificateThumbprint)] = Guid.NewGuid().ToString();

            // Act
            var result = concentratorTwinMapper.CreateDeviceDetails(twin);

            // Assert
            Assert.IsNotNull(result);

            Assert.IsFalse(result.IsConnected);
            Assert.IsFalse(result.IsEnabled);

            Assert.AreEqual(twin.Tags[nameof(Concentrator.DeviceFriendlyName).ToCamelCase()].ToString(), result.DeviceFriendlyName);
            Assert.AreEqual(twin.Tags[nameof(Concentrator.LoraRegion).ToCamelCase()].ToString(), result.LoraRegion);
            Assert.AreEqual(twin.Tags[nameof(Concentrator.DeviceType).ToCamelCase()].ToString(), result.DeviceType);

            Assert.IsTrue(result.AlreadyLoggedInOnce);

            Assert.AreEqual(twin.Properties.Desired[nameof(Concentrator.ClientCertificateThumbprint)].ToString(), result.ClientCertificateThumbprint);
            this.mockRepository.VerifyAll();
        }

        [Test]
        public async Task UpdateTwin_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var concentratorTwinMapper = this.CreateConcentratorTwinMapper();
            Concentrator item = new Concentrator
            {
                LoraRegion = Guid.NewGuid().ToString(),
                DeviceFriendlyName = Guid.NewGuid().ToString(),
                DeviceType = Guid.NewGuid().ToString(),
                ClientCertificateThumbprint = Guid.NewGuid().ToString(),
                IsConnected = false,
                IsEnabled = false,
                AlreadyLoggedInOnce = false,
            };

            using var deviceResponseMock = new HttpResponseMessage();

            deviceResponseMock.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            this.httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage req, CancellationToken token) =>
                {
                    if (req.RequestUri.LocalPath.Equals($"/{item.LoraRegion}.json", StringComparison.OrdinalIgnoreCase))
                    {
                        return deviceResponseMock;
                    }

                    return null;
                })
                .Verifiable();


            Twin twin = new Twin();
            this.mockConfiguration.SetupGet(x => x[It.Is<string>(c => c == "LoRaRegionRouterConfig:Url")])
                .Returns("http://fake.local");
            
            Helpers.DeviceHelper.SetTagValue(twin, nameof(item.DeviceFriendlyName), item.DeviceFriendlyName);
            Helpers.DeviceHelper.SetTagValue(twin, nameof(item.DeviceType), item.DeviceType);
            Helpers.DeviceHelper.SetTagValue(twin, nameof(item.LoraRegion), item.LoraRegion);

            twin.Properties.Desired[nameof(Concentrator.ClientCertificateThumbprint)] = item.ClientCertificateThumbprint;

            // Act
            await concentratorTwinMapper.UpdateTwin(twin, item);

            // Assert
            Assert.AreEqual(item.DeviceFriendlyName, twin.Tags[nameof(Concentrator.DeviceFriendlyName).ToCamelCase()].ToString());
            Assert.AreEqual(item.DeviceType, twin.Tags[nameof(Concentrator.DeviceType).ToCamelCase()].ToString());
            Assert.AreEqual(item.LoraRegion, twin.Tags[nameof(Concentrator.LoraRegion).ToCamelCase()].ToString());

            Assert.AreEqual(item.ClientCertificateThumbprint, twin.Properties.Desired[nameof(Concentrator.ClientCertificateThumbprint)].ToString());

            this.mockRepository.VerifyAll();
        }
    }
}
