// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Tests.Unit.Client.Pages.Devices
{
    using System;
    using System.Collections.Generic;
    using AzureIoTHub.Portal.Client.Pages.Devices;
    using AzureIoTHub.Portal.Client.Services;
    using Models.v10;
    using UnitTests.Bases;
    using Bunit;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using MudBlazor.Services;
    using NUnit.Framework;
    using UnitTests.Mocks;
    using AutoFixture;
    using AzureIoTHub.Portal.Shared.Models.v10.Filters;
    using System.Linq;
    using AzureIoTHub.Portal.Shared.Models;
    using AzureIoTHub.Portal.Shared.Constants;

    [TestFixture]
    public class CreateDevicePageTests : BlazorUnitTest
    {
        private Mock<IDeviceTagSettingsClientService> mockDeviceTagSettingsClientService;
        private Mock<ILoRaWanDeviceClientService> mockLoRaWanDeviceClientService;
        private Mock<IDeviceClientService> mockDeviceClientService;
        private Mock<ILoRaWanDeviceModelsClientService> mockLoRaWanDeviceModelsClientService;
        private Mock<IDeviceModelsClientService> mockDeviceModelsClientService;

        public override void Setup()
        {
            base.Setup();

            this.mockDeviceTagSettingsClientService = MockRepository.Create<IDeviceTagSettingsClientService>();
            this.mockLoRaWanDeviceClientService = MockRepository.Create<ILoRaWanDeviceClientService>();
            this.mockDeviceClientService = MockRepository.Create<IDeviceClientService>();
            this.mockLoRaWanDeviceModelsClientService = MockRepository.Create<ILoRaWanDeviceModelsClientService>();
            this.mockDeviceModelsClientService = MockRepository.Create<IDeviceModelsClientService>();

            _ = Services.AddSingleton(this.mockDeviceTagSettingsClientService.Object);
            _ = Services.AddSingleton(this.mockLoRaWanDeviceClientService.Object);
            _ = Services.AddSingleton(new PortalSettings { CloudProvider = CloudProviders.Azure });

            _ = Services.AddSingleton<IDeviceLayoutService, DeviceLayoutService>();

            Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));
        }

        private void PortalSettingsCloudProvider(string cloudProvider)
        {
            Services.GetRequiredService<PortalSettings>().CloudProvider = cloudProvider;
        }

        [Test]
        public void CreateDevicePageShouldRenderCorrectly()
        {
            var mockDeviceModel = new DeviceModelDto
            {
                ModelId = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                SupportLoRaFeatures = false,
                Name = Guid.NewGuid().ToString()
            };

            var expectedDeviceDetails = new DeviceDetails
            {
                DeviceName = Guid.NewGuid().ToString(),
                ModelId = mockDeviceModel.ModelId,
                DeviceID = Guid.NewGuid().ToString(),
            };

            _ = this.mockDeviceClientService.Setup(service => service.CreateDevice(It.Is<DeviceDetails>(details => expectedDeviceDetails.DeviceID.Equals(details.DeviceID, StringComparison.Ordinal))))
                .Returns(Task.FromResult(expectedDeviceDetails.DeviceID));

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTagDto>
                {
                    new()
                    {
                        Label = Guid.NewGuid().ToString(),
                        Name = Guid.NewGuid().ToString(),
                        Required = false,
                        Searchable = false
                    }
                });

            // Act
            var cut = RenderComponent<CreateDevicePage>();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void OnInitializedAsyncShouldProcessProblemDetailsExceptionWhenIssueOccursOnGettingDeviceTags()
        {

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            // Act
            var cut = RenderComponent<CreateDevicePage>();

            // Assert
            cut.WaitForAssertion(() => cut.Markup.Should().NotBeNullOrEmpty());
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public async Task SaveShouldProcessProblemDetailsExceptionWhenIssueOccursOnCreatingDevice()
        {
            var mockDeviceModel = new DeviceModelDto
            {
                ModelId = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                SupportLoRaFeatures = false,
                Name = Guid.NewGuid().ToString()
            };

            var expectedDeviceDetails = new DeviceDetails
            {
                DeviceName = Guid.NewGuid().ToString(),
                ModelId = mockDeviceModel.ModelId,
                DeviceID = Guid.NewGuid().ToString(),
            };

            _ = this.mockDeviceClientService.Setup(service => service.CreateDevice(It.Is<DeviceDetails>(details => expectedDeviceDetails.DeviceID.Equals(details.DeviceID, StringComparison.Ordinal))))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTagDto>
                {
                    new()
                    {
                        Label = Guid.NewGuid().ToString(),
                        Name = Guid.NewGuid().ToString(),
                        Required = false,
                        Searchable = false
                    }
                });

            _ = this.mockDeviceModelsClientService
                .Setup(service => service.GetDeviceModelModelProperties(mockDeviceModel.ModelId))
                .ReturnsAsync(new List<DeviceProperty>());

            // Act
            var cut = RenderComponent<CreateDevicePage>();
            var saveButton = cut.WaitForElement("#SaveButton");

            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceName)}").Change(expectedDeviceDetails.DeviceName);
            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceID)}").Change(expectedDeviceDetails.DeviceID);
            await cut.Instance.ChangeModel(mockDeviceModel);

            saveButton.Click();

            // Assert
            cut.WaitForAssertion(() => this.mockNavigationManager.Uri.Should().NotEndWith("devices"));
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public async Task ChangeModelShouldProcessProblemDetailsExceptionWhenIssueOccursOnGettingModelProperties()
        {
            var mockDeviceModel = new DeviceModelDto
            {
                ModelId = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                SupportLoRaFeatures = false,
                Name = Guid.NewGuid().ToString()
            };

            var expectedDeviceDetails = new DeviceDetails
            {
                DeviceName = Guid.NewGuid().ToString(),
                ModelId = mockDeviceModel.ModelId,
                DeviceID = Guid.NewGuid().ToString(),
            };

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTagDto>
                {
                    new()
                    {
                        Label = Guid.NewGuid().ToString(),
                        Name = Guid.NewGuid().ToString(),
                        Required = false,
                        Searchable = false
                    }
                });

            _ = this.mockDeviceModelsClientService
                    .Setup(service => service.GetDeviceModelModelProperties(mockDeviceModel.ModelId))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            var cut = RenderComponent<CreateDevicePage>();

            // Act
            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceName)}").Change(expectedDeviceDetails.DeviceName);
            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceID)}").Change(expectedDeviceDetails.DeviceID);
            await cut.Instance.ChangeModel(mockDeviceModel);

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public async Task ClickOnSaveAndAddNewShouldCreateDeviceAndResetCreateDevicePage()
        {
            var mockDeviceModel = new DeviceModelDto
            {
                ModelId = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                SupportLoRaFeatures = false,
                Name = Guid.NewGuid().ToString()
            };

            var expectedDeviceDetails = new DeviceDetails
            {
                DeviceName = Guid.NewGuid().ToString(),
                ModelId = mockDeviceModel.ModelId,
                DeviceID = Guid.NewGuid().ToString(),
            };

            _ = this.mockDeviceClientService.Setup(service => service.CreateDevice(It.Is<DeviceDetails>(details => expectedDeviceDetails.DeviceID.Equals(details.DeviceID, StringComparison.Ordinal))))
                .Returns(Task.FromResult(expectedDeviceDetails.DeviceID));

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTagDto>
                {
                    new()
                    {
                        Label = Guid.NewGuid().ToString(),
                        Name = Guid.NewGuid().ToString(),
                        Required = false,
                        Searchable = false
                    }
                });

            _ = this.mockDeviceModelsClientService
                .Setup(service => service.GetDeviceModelModelProperties(mockDeviceModel.ModelId))
                .ReturnsAsync(new List<DeviceProperty>());

            _ = this.mockDeviceClientService
                .Setup(service => service.SetDeviceProperties(expectedDeviceDetails.DeviceID, It.IsAny<IList<DevicePropertyValue>>()))
                .Returns(Task.CompletedTask);

            var popoverProvider = RenderComponent<MudPopoverProvider>();
            var cut = RenderComponent<CreateDevicePage>();
            var saveButton = cut.WaitForElement("#SaveButton");

            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceName)}").Change(expectedDeviceDetails.DeviceName);
            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceID)}").Change(expectedDeviceDetails.DeviceID);
            await cut.Instance.ChangeModel(mockDeviceModel);

            var mudButtonGroup = cut.FindComponent<MudButtonGroup>();

            mudButtonGroup.Find(".mud-menu button").Click();

            popoverProvider.WaitForAssertion(() => popoverProvider.FindAll("div.mud-list-item").Count.Should().Be(3));

            var items = popoverProvider.FindAll("div.mud-list-item");

            // Click on Save and New
            items[1].Click();

            // Act
            saveButton.Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
            cut.WaitForAssertion(() => cut.Find($"#{nameof(DeviceDetails.DeviceName)}").TextContent.Should().BeEmpty());
            cut.WaitForAssertion(() => cut.Find($"#{nameof(DeviceDetails.DeviceID)}").TextContent.Should().BeEmpty());
            cut.WaitForAssertion(() => this.mockNavigationManager.Uri.Should().NotEndWith("/devices"));
        }

        [Test]
        public async Task ClickOnSaveAndDuplicateShouldCreateDeviceAndDuplicateDeviceDetailsInCreateDevicePage()
        {
            var mockDeviceModel = new DeviceModelDto
            {
                ModelId = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                SupportLoRaFeatures = false,
                Name = Guid.NewGuid().ToString()
            };

            var expectedDeviceDetails = new DeviceDetails
            {
                DeviceName = Guid.NewGuid().ToString(),
                ModelId = mockDeviceModel.ModelId,
                DeviceID = Guid.NewGuid().ToString(),
            };

            _ = this.mockDeviceClientService.Setup(service => service.CreateDevice(It.Is<DeviceDetails>(details => expectedDeviceDetails.DeviceID.Equals(details.DeviceID, StringComparison.Ordinal))))
                .Returns(Task.FromResult(expectedDeviceDetails.DeviceID));

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTagDto>
                {
                    new()
                    {
                        Label = Guid.NewGuid().ToString(),
                        Name = Guid.NewGuid().ToString(),
                        Required = false,
                        Searchable = false
                    }
                });

            _ = this.mockDeviceModelsClientService
                .Setup(service => service.GetDeviceModelModelProperties(mockDeviceModel.ModelId))
                .ReturnsAsync(new List<DeviceProperty>());

            _ = this.mockDeviceClientService
                .Setup(service => service.SetDeviceProperties(expectedDeviceDetails.DeviceID, It.IsAny<IList<DevicePropertyValue>>()))
                .Returns(Task.CompletedTask);

            var popoverProvider = RenderComponent<MudPopoverProvider>();
            var cut = RenderComponent<CreateDevicePage>();
            var saveButton = cut.WaitForElement("#SaveButton");

            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceName)}").Change(expectedDeviceDetails.DeviceName);
            cut.WaitForElement($"#{nameof(DeviceDetails.DeviceID)}").Change(expectedDeviceDetails.DeviceID);
            await cut.Instance.ChangeModel(mockDeviceModel);

            var mudButtonGroup = cut.FindComponent<MudButtonGroup>();

            mudButtonGroup.Find(".mud-menu button").Click();

            popoverProvider.WaitForAssertion(() => popoverProvider.FindAll("div.mud-list-item").Count.Should().Be(3));

            var items = popoverProvider.FindAll("div.mud-list-item");

            // Click on Save and Duplicate
            items[2].Click();

            // Act
            saveButton.Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
            cut.WaitForAssertion(() => cut.Find($"#{nameof(DeviceDetails.DeviceName)}").TextContent.Should().BeEmpty());
            cut.WaitForAssertion(() => cut.Find($"#{nameof(DeviceDetails.DeviceID)}").TextContent.Should().BeEmpty());
            cut.WaitForAssertion(() => this.mockNavigationManager.Uri.Should().NotEndWith("/devices"));
        }

        [Test]
        public void SearchDeviceModels_InputExisingDeviceModelName_DeviceModelReturned()
        {
            // Arrange
            var deviceModels = Fixture.CreateMany<DeviceModelDto>(2).ToList();
            var expectedDeviceModel = deviceModels.First();

            _ = this.mockDeviceModelsClientService.Setup(service => service.GetDeviceModels(It.Is<DeviceModelFilter>(x => expectedDeviceModel.Name.Equals(x.SearchText))))
                .ReturnsAsync(new PaginationResult<DeviceModelDto>
                {
                    Items = deviceModels.Where(x => expectedDeviceModel.Name.Equals(x.Name, StringComparison.Ordinal))
                });

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTagDto>());

            var popoverProvider = RenderComponent<MudPopoverProvider>();

            var cut = RenderComponent<CreateDevicePage>();

            var autocompleteComponent = cut.FindComponent<MudAutocomplete<IDeviceModel>>();

            // Act
            autocompleteComponent.Find("input").Input(expectedDeviceModel.Name);

            // Assert
            popoverProvider.WaitForAssertion(() => popoverProvider.FindAll("div.mud-popover-open").Count.Should().Be(1));
            popoverProvider.WaitForAssertion(() => popoverProvider.FindAll("div.mud-list-item").Count.Should().Be(1));

            var items = popoverProvider.FindComponents<MudListItem>().ToArray();
            _ = items.Length.Should().Be(1);
            _ = items.First().Markup.Should().Contain(expectedDeviceModel.Name);
            items.First().Find("div.mud-list-item").Click();

            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public Task AWSCreateDeviceShouldNotHaveDeviceID()
        {
            // Arrange
            PortalSettingsCloudProvider(CloudProviders.AWS);

            var mockDeviceModel = new DeviceModelDto
            {
                ModelId = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                SupportLoRaFeatures = false,
                Name = Guid.NewGuid().ToString()
            };

            _ = this.mockDeviceTagSettingsClientService.Setup(service => service.GetDeviceTags())
                .ReturnsAsync(new List<DeviceTagDto>
                {
                    new()
                    {
                        Label = Guid.NewGuid().ToString(),
                        Name = Guid.NewGuid().ToString(),
                        Required = false,
                        Searchable = false
                    }
                });

            _ = this.mockDeviceModelsClientService
               .Setup(service => service.GetDeviceModelModelProperties(mockDeviceModel.ModelId))
               .ReturnsAsync(new List<DeviceProperty>());

            var cut = RenderComponent<CreateDevicePage>();

            // Act
            var result = () => cut.Find($"#{nameof(DeviceDetails.DeviceID)}");

            // Assert
            _ = result.Should().Throw<ElementNotFoundException>();
            return Task.CompletedTask;
        }
    }
}
