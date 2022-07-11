// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Tests.Unit.Pages.DeviceConfigurations
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Bunit;
    using Client.Exceptions;
    using Client.Models;
    using Client.Pages.DeviceConfigurations;
    using Client.Services;
    using FluentAssertions;
    using Helpers;
    using Microsoft.Extensions.DependencyInjection;
    using Models.v10;
    using Moq;
    using MudBlazor;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;

    [TestFixture]
    public class CreateDeviceConfigurationsPageTests : BlazorUnitTest
    {
        private Mock<IDialogService> mockDialogService;
        private Mock<IDeviceConfigurationsClientService> mockDeviceConfigurationsClientService;
        private Mock<IDeviceModelsClientService> mockDeviceModelsClientService;
        private Mock<IDeviceTagSettingsClientService> mockDeviceTagSettingsClientService;

        public override void Setup()
        {
            base.Setup();

            this.mockDialogService = MockRepository.Create<IDialogService>();
            this.mockDeviceConfigurationsClientService = MockRepository.Create<IDeviceConfigurationsClientService>();
            this.mockDeviceModelsClientService = MockRepository.Create<IDeviceModelsClientService>();
            this.mockDeviceTagSettingsClientService = MockRepository.Create<IDeviceTagSettingsClientService>();

            _ = Services.AddSingleton(this.mockDialogService.Object);
            _ = Services.AddSingleton(this.mockDeviceConfigurationsClientService.Object);
            _ = Services.AddSingleton(this.mockDeviceModelsClientService.Object);
            _ = Services.AddSingleton(this.mockDeviceTagSettingsClientService.Object);
        }

        [Test]
        public void DeviceConfigurationDetailPageShouldRenderCorrectly()
        {
            // Arrange

            _ = this.mockDeviceModelsClientService.Setup(service =>
                    service.GetDeviceModels())
                .ReturnsAsync(new List<DeviceModel>());

            _ = this.mockDeviceTagSettingsClientService.Setup(service =>
                    service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTag>());

            // Act
            var cut = RenderComponent<CreateDeviceConfigurationsPage>();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void DeviceConfigurationDetailShouldCreateConfiguration()
        {
            // Arrange
            var configuration = new DeviceConfig
            {
                ConfigurationId = Guid.NewGuid().ToString(),
                ModelId = Guid.NewGuid().ToString(),
                Priority = 1,
                Tags = new Dictionary<string, string>(),
                Properties = new Dictionary<string, string>()
            };

            _ = this.mockDeviceModelsClientService.Setup(service =>
                    service.GetDeviceModels())
                .ReturnsAsync(new List<DeviceModel>());

            _ = this.mockDeviceTagSettingsClientService.Setup(service =>
                    service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTag>());

            _ = this.mockDeviceConfigurationsClientService.Setup(service =>
                    service.CreateDeviceConfiguration(It.Is<DeviceConfig>(config => configuration.Equals(config))))
                .Returns(Task.CompletedTask);

            var cut = RenderComponent<CreateDeviceConfigurationsPage>();
            cut.Instance.Configuration = configuration;


            // Act
            cut.WaitForElement("#saveButton").Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void DeviceConfigurationDetailShouldProcessProblemDetailsExceptionWhenIssueOccursOnCreatingConfiguration()
        {
            // Arrange
            var configuration = new DeviceConfig
            {
                ConfigurationId = Guid.NewGuid().ToString(),
                ModelId = Guid.NewGuid().ToString(),
                Priority = 1,
                Tags = new Dictionary<string, string>(),
                Properties = new Dictionary<string, string>()
            };

            _ = this.mockDeviceModelsClientService.Setup(service =>
                    service.GetDeviceModels())
                .ReturnsAsync(new List<DeviceModel>());

            _ = this.mockDeviceTagSettingsClientService.Setup(service =>
                    service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTag>());

            _ = this.mockDeviceConfigurationsClientService.Setup(service =>
                    service.CreateDeviceConfiguration(It.Is<DeviceConfig>(config => configuration.Equals(config))))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            var cut = RenderComponent<CreateDeviceConfigurationsPage>();
            cut.Instance.Configuration = configuration;


            // Act
            cut.WaitForElement("#saveButton").Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void DeviceConfigurationDetailPageShouldProcessProblemDetailsExceptionWhenIssueOccursOnLoadingModels()
        {
            // Arrange
            _ = this.mockDeviceModelsClientService.Setup(service =>
                    service.GetDeviceModels())
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            // Act
            var cut = RenderComponent<CreateDeviceConfigurationsPage>();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void DeviceConfigurationDetailPageShouldProcessProblemDetailsExceptionWhenIssueOccursOnLoadingTags()
        {
            // Arrange
            _ = this.mockDeviceModelsClientService.Setup(service =>
                    service.GetDeviceModels())
                .ReturnsAsync(new List<DeviceModel>());

            _ = this.mockDeviceTagSettingsClientService.Setup(service =>
                    service.GetDeviceTags())
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            // Act
            var cut = RenderComponent<CreateDeviceConfigurationsPage>();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void WhenClickToDeleteTagShouldRemoveTheSelectedTag()
        {
            // Arrange
            _ = this.mockDeviceModelsClientService.Setup(service =>
                    service.GetDeviceModels())
                .ReturnsAsync(new List<DeviceModel>());

            _ = this.mockDeviceTagSettingsClientService.Setup(service =>
                    service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTag>
                {
                    new () { Name = "tag0" },
                    new () { Name = "tag1" }
                });

            var cut = RenderComponent<CreateDeviceConfigurationsPage>();

            cut.Instance.SelectedTag = "tag0";
            cut.Render();
            cut.WaitForElement("#addTagButton").Click();

            cut.Instance.SelectedTag = "tag1";
            cut.Render();
            cut.WaitForElement("#addTagButton").Click();

            // Act
            cut.WaitForElement("#tag-tag1 #deleteTagButton").Click();

            // Assert
            _ = cut.Instance.Configuration.Tags.Count.Should().Be(1);
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void WhenClickToAddTagShouldAddTheSelectedTag()
        {
            // Arrange
            var modelId = Guid.NewGuid().ToString();

            _ = this.mockDeviceModelsClientService.Setup(service =>
                    service.GetDeviceModels())
                .ReturnsAsync(new List<DeviceModel>
                {
                    new ()
                    {
                        ModelId = modelId,
                        Name = Guid.NewGuid().ToString()
                    }
                });

            _ = MockHttpClient
                .When(HttpMethod.Get, $"/api/models/{modelId}/properties")
                .RespondJson(Array.Empty<DeviceProperty>());

            _ = this.mockDeviceTagSettingsClientService.Setup(service =>
                    service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTag>
                {
                    new () { Name = "tag0" },
                    new () { Name = "tag1" }
                });

            var cut = RenderComponent<CreateDeviceConfigurationsPage>();

            // Act
            cut.Instance.SelectedTag = "tag1";
            cut.Render();
            cut.WaitForElement("#addTagButton").Click();

            // Assert
            _ = cut.WaitForElement("#tag-tag1");
            _ = cut.Instance.Configuration.Tags.Count.Should().Be(1);
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }
    }
}
