// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AzureIoTHub.Portal.Server.Tests.Unit.Pages.LoRaWan.Concentrator
{
    using System;
    using System.Threading.Tasks;
    using AzureIoTHub.Portal.Client.Pages.LoRaWAN.Concentrator;
    using AzureIoTHub.Portal.Client.Shared;
    using Models.v10;
    using Models.v10.LoRaWAN;
    using Bunit;
    using Bunit.TestDoubles;
    using Client.Exceptions;
    using Client.Models;
    using Client.Services;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Mocks;
    using Moq;
    using MudBlazor;
    using MudBlazor.Services;
    using NUnit.Framework;

    [TestFixture]
    public class ConcentratorDetailPageTests : BlazorUnitTest
    {
        private Mock<IDialogService> mockDialogService;
        private Mock<ILoRaWanConcentratorsClientService> mockLoRaWanConcentratorsClientService;

        private readonly string mockDeviceId = Guid.NewGuid().ToString();

        private FakeNavigationManager mockNavigationManager;

        public override void Setup()
        {
            base.Setup();

            this.mockDialogService = MockRepository.Create<IDialogService>();
            this.mockLoRaWanConcentratorsClientService = MockRepository.Create<ILoRaWanConcentratorsClientService>();

            _ = Services.AddSingleton(this.mockDialogService.Object);
            _ = Services.AddSingleton(this.mockLoRaWanConcentratorsClientService.Object);
            _ = Services.AddSingleton(new PortalSettings { IsLoRaSupported = true });

            Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            this.mockNavigationManager = Services.GetRequiredService<FakeNavigationManager>();
        }

        [Test]
        public void ReturnButtonMustNavigateToPreviousPage()
        {
            // Arrange
            _ = this.mockLoRaWanConcentratorsClientService.Setup(service => service.GetConcentrator(this.mockDeviceId))
                .ReturnsAsync(new Concentrator());

            var cut = RenderComponent<ConcentratorDetailPage>(ComponentParameter.CreateParameter("DeviceID", this.mockDeviceId));
            cut.WaitForAssertion(() => cut.Find("#returnButton"));

            // Act
            cut.Find("#returnButton").Click();

            // Assert
            cut.WaitForAssertion(() => this.mockNavigationManager.Uri.Should().EndWith("/lorawan/concentrators"));
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ConcentratorDetailPageShouldProcessProblemDetailsExceptionWhenIssueOccursOnGettingConcentratorDetails()
        {
            // Arrange
            _ = this.mockLoRaWanConcentratorsClientService.Setup(service => service.GetConcentrator(this.mockDeviceId))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            // Act
            var cut = RenderComponent<ConcentratorDetailPage>(ComponentParameter.CreateParameter("DeviceID", this.mockDeviceId));
            cut.WaitForAssertion(() => cut.Find("#returnButton"));

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnSaveShouldPutConcentratorDetails()
        {
            // Arrange
            var mockConcentrator = new Concentrator()
            {
                DeviceId = "1234567890123456",
                DeviceName = Guid.NewGuid().ToString(),
                LoraRegion = Guid.NewGuid().ToString()
            };

            _ = this.mockLoRaWanConcentratorsClientService.Setup(service => service.GetConcentrator(mockConcentrator.DeviceId))
                .ReturnsAsync(mockConcentrator);

            _ = this.mockLoRaWanConcentratorsClientService
                .Setup(service => service.UpdateConcentrator(mockConcentrator))
                .Returns(Task.CompletedTask);

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);

            _ = this.mockDialogService.Setup(c => c.Show<ProcessingDialog>("Processing", It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);

            _ = this.mockDialogService.Setup(c => c.Close(It.Is<DialogReference>(x => x == mockDialogReference)));

            var cut = RenderComponent<ConcentratorDetailPage>(ComponentParameter.CreateParameter("DeviceID", mockConcentrator.DeviceId));
            cut.WaitForAssertion(() => cut.Find("#saveButton"));

            // Act
            cut.Find("#saveButton").Click();

            // Assert
            cut.WaitForAssertion(() => this.mockNavigationManager.Uri.Should().EndWith("/lorawan/concentrators"));
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }

        [Test]
        public void ClickOnSaveShouldProcessProblemDetailsExceptionWhenIssueOccursOnUpdatingConcentratorDetails()
        {
            // Arrange
            var mockConcentrator = new Concentrator()
            {
                DeviceId = "1234567890123456",
                DeviceName = Guid.NewGuid().ToString(),
                LoraRegion = Guid.NewGuid().ToString()
            };

            _ = this.mockLoRaWanConcentratorsClientService.Setup(service => service.GetConcentrator(mockConcentrator.DeviceId))
                .ReturnsAsync(mockConcentrator);

            _ = this.mockLoRaWanConcentratorsClientService
                .Setup(service => service.UpdateConcentrator(mockConcentrator))
                .ThrowsAsync(new ProblemDetailsException(new ProblemDetailsWithExceptionDetails()));

            var mockDialogReference = new DialogReference(Guid.NewGuid(), this.mockDialogService.Object);

            _ = this.mockDialogService.Setup(c => c.Show<ProcessingDialog>("Processing", It.IsAny<DialogParameters>()))
                .Returns(mockDialogReference);

            _ = this.mockDialogService.Setup(c => c.Close(It.Is<DialogReference>(x => x == mockDialogReference)));

            var cut = RenderComponent<ConcentratorDetailPage>(ComponentParameter.CreateParameter("DeviceID", mockConcentrator.DeviceId));
            cut.WaitForAssertion(() => cut.Find("#saveButton"));

            // Act
            cut.Find("#saveButton").Click();

            // Assert
            cut.WaitForAssertion(() => MockRepository.VerifyAll());
        }
    }
}
